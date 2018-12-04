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
            // Initialize the driver for malos vision with the IP and the BasePort.
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

VisionResult
============

This message organizes (at least) four components:
- Detections (faces, hands, cars).
- Tracking information.
- Recognition (gender, age, emotion).
- Events (New face in video, face leaves video, gesture starts).

**rectDetection**
Results of all the rectangular detectors.
Each detecion also stores the recognitions that can be done inside of this rectangle. For instance, for faces the available recognitions are (age, gender, emotion).

**visionEvent**
Vision events. For instance, tracking events (start, end).
This message is not inside rect_detection because some events will happen when the detection is no longer available, for instance: TRACKING_END.

**uuid**
UUID (v4) that uniquely identifies each vision result

RectangularDetection
====================
Result of a rectangle detector.

**facialRecognition**
Facial recognitions for this detection (age, gender, pose, features, etc).

**tag**
What kind of detections the rectangle contains.

**trackingId**
Tracking id for this detection.

**uuid**
UUID (v4) identifies this unique rectangular detection


FacialRecognition
=================

**The following fields should only be present when the tag HAS_AGE tag is set.**

**age**
Detected age.



**The following fields should only be present when the tag HAS_GENDER is set.**

**gender**
Detected gender, genders available for detections are MALE, FEAMLE.



**The following fields should only be present when the tag HAS_EMOTION is set.**

**emotion**
Detected emotion, emotions available for detections are ANGRY, DISGUST, CONFUSED, HAPPY, SAD, SURPRISED, CALM, FEAR, NEUTRAL.



**The following fields should only be present when the tag HAS_HEAD_POSE is set.**

**poseYaw**
Face yaw.

**poseRoll**
Face roll.

**posePitch**
Face pitch.

**isLooking**
Face looking.


VisionEvent
===========

**tag**
Tag for events. The fields below will make sense for a specific tag. for face will be TRACKING_START or TRACKING_END

**trackingId**
Object identifier.

**dwellTime**
Dwell time: Amount of seconds facing the camera. Used for TRACKING_END event.


Who can answer questions about this library?
============================================

- Leonardo Vernaza <leonardo.vernaza@admobilize.com>
- Maciej Ruckgaber <maciek.ruckgaber@admobilize.com>


.. _0MQ: http://zeromq.org/
.. _MATRIX-MALOS services: https://matrix-io.github.io/matrix-documentation/matrix-core/getting-started/understanding-core/
