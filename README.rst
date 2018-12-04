.. image:: https://img.shields.io/nuget/v/matrix-io-malos.svg?style=flat-square
    :target: https://www.nuget.org/packages/matrix-io-malos/
.. image:: https://admobilize.visualstudio.com/MATRIX/_apis/build/status/matrix-io-malos
    :target: https://admobilize.visualstudio.com/MATRIX/_build/latest?definitionId=35
============================
MATRIXIO .Net MALOS Driver
============================
A simple .Net driver for communicating with `MATRIX-MALOS services`_.

License
=======

This application follows the GNU General Public License, as described in the ``LICENSE`` file.

Installing
==========

The package is available on NuGet, so you can easily install via NuGet:

.. code-block:: console

    PM> Install-Package matrix-io-malos


Using the MalosDriver
=====================

To use the MALOS driver in your code 
you can do the following:

.. code-block:: c#

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

Who can answer questions about this library?
============================================

- Leonardo Vernaza <leonardo.vernaza@admobilize.com>
- Maciej Ruckgaber <maciek.ruckgaber@admobilize.com>


.. _0MQ: http://zeromq.org/
.. _MATRIX-MALOS services: https://matrix-io.github.io/matrix-documentation/matrix-core/getting-started/understanding-core/
