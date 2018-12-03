using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace example
{
    class Program
    {
        static void Main(string[] args)
        {
            matrix_io_malos.Driver driver = new matrix_io_malos.Driver("127.0.0.1", matrix_io_malos.BasePort.Vision);
            driver.onGetData += Driver_onGetData;
            driver.getData();
        }

        private static void Driver_onGetData(byte[] data)
        {
            MatrixIO.Vision.V1.VisionResult result;
            result = MatrixIO.Vision.V1.VisionResult.Parser.ParseFrom(data);
        }
    }
}
