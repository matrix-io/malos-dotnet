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

MALOS Vision data
=====================
On each onGetData event you will receive a byte array data that you can parse to a VisionResult object.
if you convert the VisionResult to a Json you will have an object like the follow

.. code-block:: json

    {
        "rectDetection": [{
            "facialRecognition": [{
                "tag": "FACE_ID",
                "faceId": "1543956668-1505437196092-26970"
            }, {
                "tag": "AGE",
                "age": 38
            }, {
                "tag": "EMOTION",
                "emotion": "NEUTRAL"
            }, {
                "tag": "GENDER",
                "gender": "MALE"
            }, {
                "tag": "HEAD_POSE",
                "poseYaw": 0.0326323733,
                "posePitch": -0.0489135,
                "isLooking": true
            }],
            "trackingId": "219",
            "uuid": "30a5dbce-c27a-46d7-8334-2d7a60d427bb"
        }, {
            "facialRecognition": [{
                "tag": "FACE_ID",
                "faceId": "1543956673-1505449935887-18600"
            }, {
                "tag": "AGE",
                "age": 34
            }, {
                "tag": "EMOTION",
                "emotion": "SAD"
            }, {
                "tag": "GENDER",
                "gender": "MALE"
            }, {
                "tag": "HEAD_POSE",
                "poseYaw": 0.0282833464,
                "poseRoll": 0.05874507,
                "posePitch": 0.19542487
            }],
            "trackingId": "225",
            "uuid": "6465747a-3d69-40dd-b59f-dd51c9ebf526"
        }],
        "visionEvent": [{
            "tag": "TRACKING_END",
            "trackingId": "219",
            "dwellTime": 1.47886264
        }, {
            "tag": "TRACKING_START",
            "trackingId": "225"
        }],
        "uuid": "945968ad-aa9f-48ea-b57b-75c5b50081ce"
    }


Who can answer questions about this library?
============================================

- Leonardo Vernaza <leonardo.vernaza@admobilize.com>
- Maciej Ruckgaber <maciek.ruckgaber@admobilize.com>


.. _0MQ: http://zeromq.org/
.. _MATRIX-MALOS services: https://matrix-io.github.io/matrix-documentation/matrix-core/getting-started/understanding-core/
