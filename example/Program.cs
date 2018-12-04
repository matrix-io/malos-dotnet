using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace example
{
    class Program
    {
        static matrix_io_malos.Driver driver;

        static void Main(string[] args)
        {
            // Initialize the drive for malos vision.
            driver = new matrix_io_malos.Driver("127.0.0.1", matrix_io_malos.BasePort.Vision);

            // Add the event handler to receive the data events.
            driver.onGetData += Driver_onGetData;

            // Start the connection to get the data events from Malos Vision.
            driver.getData();
        }

        private static void Driver_onGetData(byte[] data)
        {
            // Parse the date from byte array to a vision result object.
            MatrixIO.Vision.V1.VisionResult result;
            result = MatrixIO.Vision.V1.VisionResult.Parser.ParseFrom(data);

            // Convert the vision result to a json.
            var json = Google.Protobuf.JsonFormatter.Default.Format(result);
            Console.WriteLine(json);
        }

        private static void stopGetData()
        {
            // Stop the connection to get data from Malos Vision.
            driver.stopGetData();
        }
    }
}
