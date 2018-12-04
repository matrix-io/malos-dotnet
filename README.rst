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

    PM> Install-Package matrix-io-proto


Using the MalosDriver
=====================

To use the MALOS driver works in your code 
you can do the following:

.. code-block:: c#

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

Who can answer questions about this library?
============================================

- Leonardo Vernaza <leonardo.vernaza@admobilize.com>
- Maciej Ruckgaber <maciek.ruckgaber@admobilize.com>


.. _0MQ: http://zeromq.org/
.. _MATRIX-MALOS services: https://matrix-io.github.io/matrix-documentation/matrix-core/getting-started/understanding-core/
