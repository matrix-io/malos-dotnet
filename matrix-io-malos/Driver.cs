using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace matrix_io_malos
{
    public delegate void GetData(byte[] data);
    public delegate void GetStatus(MatrixIO.Malos.Driver.V1.Status status);

    public enum BasePort { Imu = 1, Humidity, Everloop, Pressure, UV, Mic, Vision };

    public class Driver
    {
        /// <summary>
        /// Event to receive the data from MALOS.
        /// </summary>
        public event GetData onGetData;

        /// <summary>
        /// Event to receive the satus message.
        /// </summary>
        public event GetStatus onGetStatus;

        private const int imuPort = 20013;
        private const int humidityPort = 20017;
        private const int everloopPort = 20021;
        private const int pressurePort = 20025;
        private const int uvPort = 20029;
        private const int micArrayPort = 20037;
        private const int visionPort = 60001;

        private string address;
        private int basePort;
        private bool getStatusRunning = false;
        private bool getDataRunning = false;
        private bool keepAliveRunning = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address">IP address of the device exposing the MALOS 0MQ sockets</param>
        /// <param name="basePort">MALOS base port to use</param>
        public Driver(string address, BasePort basePort)
        {
            this.address = address;
            switch (basePort)
            {
                case BasePort.Imu:
                    this.basePort = imuPort;
                    break;
                case BasePort.Humidity:
                    this.basePort = humidityPort;
                    break;
                case BasePort.Everloop:
                    this.basePort = everloopPort;
                    break;
                case BasePort.Pressure:
                    this.basePort = pressurePort;
                    break;
                case BasePort.UV:
                    this.basePort = uvPort;
                    break;
                case BasePort.Mic:
                    this.basePort = micArrayPort;
                    break;
                case BasePort.Vision:
                    this.basePort = visionPort;
                    break;
            }
        }

        /// <summary>
        /// It sends the provided configuration proto, the driver.proto, to the Malos
        /// Configuration prot to configure the driver.
        /// </summary>
        /// <param name="configProto">a driver.proto containing configuration for the driver</param>
        /// <param name="timeOut">timeout in seconds to deliver configuration data to Malos</param>
        public void configure(MatrixIO.Malos.Driver.V1.DriverConfig configProto, int timeOut)
        {
            using (var context = new ZContext())
            using (var socket = new ZSocket(context, ZSocketType.PUSH))
            {
                socket.Connect(String.Format("tcp://{0}:{1}", address, basePort));
                byte[] bytes = configProto.ToByteArray();
                using (var configFrame = new ZFrame(bytes))
                {
                    socket.Send(configFrame);
                }
            }
        }

        /// <summary>
        /// Connects to the corresponding keep-alive port (basePort + 1) given the
        /// desired basePort. We send keep alive pings and receive pongs so MALOS
        /// keeps sampling the data from the sensors and we can keep yielding with gedData()
        /// </summary>
        /// <param name="delay">delay between pings in seconds</param>
        /// <param name="timeout">how long to wait for pongs before timeout in seconds</param>
        public void startKeepAlive(int delay = 5, int timeout = 5)
        {
            keepAliveRunning = true;
            using (var context = new ZContext())
            using (var socket = new ZSocket(context, ZSocketType.REQ))
            {
                socket.Connect(String.Format("tcp://{0}:{1}", address, (basePort + 1)));
                socket.SetOption(ZSocketOption.RCVTIMEO, (timeout * 1000));
                while (keepAliveRunning)
                {
                    socket.Send(new ZFrame(""));
                    using (ZFrame reply = socket.ReceiveFrame())
                    {
                        Console.WriteLine(reply.ReadString());
                        Thread.Sleep(delay * 1000);
                    }
                }
            }
        }

        /// <summary>
        /// Stops the keep alive socket.
        /// </summary>
        public void stopKeepAlive()
        {
            keepAliveRunning = false;
        }

        /// <summary>
        /// Connects to the corresponding status port (base_port +2) given the desired
        /// basePort and raise an onGetSatus event.
        /// </summary>
        public void getStatus()
        {
            getStatusRunning = true;
            using (var context = new ZContext())
            using (var socket = new ZSocket(context, ZSocketType.SUB))
            {
                socket.Connect(String.Format("tcp://{0}:{1}", address, basePort + 2));
                socket.Subscribe("");
                while (true)
                {
                    try
                    {
                        while (getStatusRunning)
                        {
                            using (ZFrame data = socket.ReceiveFrame())
                            {
                                byte[] bytes = data.Read();
                                MatrixIO.Malos.Driver.V1.Status status;
                                status = MatrixIO.Malos.Driver.V1.Status.Parser.ParseFrom(bytes);
                                onGetStatus?.Invoke(status);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("status-port: error " + ex.Message);
                    }
                    finally
                    {
                        Console.WriteLine("status-port: socket closed");
                        socket.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Stops the events to get status.
        /// </summary>
        public void stopGetStatus()
        {
            getStatusRunning = false;
        }

        /// <summary>
        /// Connects to the corresponding data port (base_port +3) and raise
        /// an onGetData event with the messages received through it.
        /// </summary>
        public void getData()
        {
            getDataRunning = true;
            using (var context = new ZContext())
            using (var socket = new ZSocket(context, ZSocketType.SUB))
            {
                socket.Connect(String.Format("tcp://{0}:{1}", address, basePort + 3));
                socket.Subscribe("");
                try
                {
                    while (getDataRunning)
                    {
                        using (ZFrame data = socket.ReceiveFrame())
                        {
                            byte[] bytes = data.Read();
                            onGetData?.Invoke(bytes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("data-port: error " + ex.Message);
                }
                finally
                {
                    Console.WriteLine("data-port: socket closed");
                    socket.Close();
                }
            }
        }

        /// <summary>
        /// Stops the events to get data.
        /// </summary>
        public void stopGetData()
        {
            getDataRunning = false;
        }
    }
}
