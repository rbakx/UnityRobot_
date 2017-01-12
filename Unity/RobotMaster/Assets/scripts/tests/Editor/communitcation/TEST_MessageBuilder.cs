using Communication;
using Communication.Messages;
using Communication.Transform;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommunicationTests
{
    [TestFixture]
    public class TEST_MessageBuilder
    {

        [Test]
        public void CreateMessage()
        {
            MessageTarget_ target = MessageTarget_.Robot;
            MessageType_ type = MessageType_.CustomMessage;

            Message result = MessageBuilder.CreateMessage(target, type);

            Assert.AreEqual(result.messageTarget, target);
            Assert.AreEqual(result.messageType, type);
        }

        [Test]
        public void CreateIdentificationResponse_()
        {
            string robotType = "Test type";

            IdentificationResponse_ result = MessageBuilder.CreateIdentificationResponse_(robotType);

            Assert.AreEqual(result.robotType, robotType);
        }

        [Test]
        public void CreateError_()
        {
            string errorMsg = "Test error Message";

            Error_ result = MessageBuilder.CreateError_(errorMsg);

            Assert.AreEqual(result.message, errorMsg);
        }

        [Test]
        public void CreateCustomMessage_()
        {
            string key = "test key";
            string data = "test data";

            CustomMessage_ result = MessageBuilder.CreateCustomMessage_(key, data);

            Assert.AreEqual(result.key, key);
            Assert.AreEqual(result.data, data);
        }

        [Test]
        public void CreateSetVelocity_()
        {
            Vector3 linear = new Vector3(1.1f, 2.2f, 3.3f);
            Vector3 angular = new Vector3(4.4f, 5.5f, 6.6f);

            SetVelocity_ result = UnityMessageBuilder.CreateSetVelocity_(linear, angular);

            Assert.AreEqual(result.linearVelocity.ToUnityVector(), linear);
            Assert.AreEqual(result.angularVelocity.ToUnityVector(), angular);
        }

        [Test]
        public void CreateSetVelocity_nullargs()
        {
            Vector3 linear = new Vector3(1.1f, 2.2f, 3.3f);
            Vector3 angular = new Vector3(4.4f, 5.5f, 6.6f);

            SetVelocity_ result = UnityMessageBuilder.CreateSetVelocity_(null, null);

            Assert.IsNull(result.linearVelocity);
            Assert.IsNull(result.angularVelocity);

            result = UnityMessageBuilder.CreateSetVelocity_(linear, null);

            Assert.AreEqual(result.linearVelocity.ToUnityVector(), linear);
            Assert.IsNull(result.angularVelocity);

            result = UnityMessageBuilder.CreateSetVelocity_(null, angular);

            Assert.IsNull(result.linearVelocity);
            Assert.AreEqual(result.angularVelocity.ToUnityVector(), angular);
        }

        [Test]
        public static void CreateVector3_floats()
        {
            float x = 1.1f;
            float y = 2.2f;
            float z = 3.3f;

            Vector3_ result = MessageBuilder.CreateVector(x, y, z);

            Assert.AreEqual(result.x, x);
            Assert.AreEqual(result.y, y);
            Assert.AreEqual(result.z, z);
        }

        [Test]
        public static void CreateVector3_unityVec()
        {
            Vector3 vec = new Vector3(1.1f, 2.2f, 3.3f);

            Vector3_ result = UnityMessageBuilder.CreateVector(vec);

            Assert.AreEqual(result.x, vec.x);
            Assert.AreEqual(result.y, vec.y);
            Assert.AreEqual(result.z, vec.z);
        }

        [Test]
        public static void CreateShape_()
        {
            int id = 42;

            List<Vector3> vecs = new List<Vector3>();

            vecs.Add(new Vector3(1, 1, 1));
            vecs.Add(new Vector3(2, 2, 2));
            vecs.Add(new Vector3(3, 3, 3));

            List<UInt32> indices = new List<UInt32>();
            indices.Add(1);
            indices.Add(0);
            indices.Add(2);

            Shape_ result = UnityMessageBuilder.CreateShape_(id, vecs, indices);

            Assert.AreEqual(result.id, id);

            Assert.AreEqual(result.vertices.Count, vecs.Count);
            for (int i = 0; i < result.vertices.Count; i++)
            {
                var protoVec = result.vertices[i];
                var unityVec = vecs[i];

                Assert.AreEqual(unityVec, protoVec.ToUnityVector());
            }

            Assert.AreEqual(indices.Count, result.indices.Count);
            for (int i = 0; i < result.indices.Count; i++)
            {
                Assert.AreEqual(indices[i], result.indices[i]);
            }

            result.vertices.RemoveAt(0);
            Assert.AreEqual(vecs.Count - 1, result.vertices.Count);
            Assert.AreEqual(vecs[1], result.vertices[0].ToUnityVector());

            result.indices.RemoveAt(0);
            Assert.AreEqual(indices.Count - 1, result.indices.Count);
            Assert.AreEqual(indices[1], result.indices[0]);

        }

        [Test]
        public static void CreateShapeUpdateInfo_()
        {
            List<Vector3> vert1 = new List<Vector3>();
            List<Vector3> vert2 = new List<Vector3>();
            List<Vector3> vert3 = new List<Vector3>();
            List<Vector3> vert4 = new List<Vector3>();

            vert1.Add(new Vector3(1, 1, 1));
            vert1.Add(new Vector3(1, 2, 2));
            vert1.Add(new Vector3(1, 3, 3));

            vert2.Add(new Vector3(2, 1, 1));
            vert2.Add(new Vector3(2, 2, 2));
            vert2.Add(new Vector3(2, 3, 3));

            vert3.Add(new Vector3(3, 1, 1));
            vert3.Add(new Vector3(3, 2, 2));
            vert3.Add(new Vector3(3, 3, 3));

            vert4.Add(new Vector3(4, 1, 1));
            vert4.Add(new Vector3(4, 2, 2));
            vert4.Add(new Vector3(4, 3, 3));

            List<UInt32> indices = new List<UInt32>();
            indices.Add(0);
            indices.Add(1);
            indices.Add(2);

            Shape_ shape1 = UnityMessageBuilder.CreateShape_(1, vert1, indices);
            Shape_ shape2 = UnityMessageBuilder.CreateShape_(2, vert2, indices);
            Shape_ shape3 = UnityMessageBuilder.CreateShape_(3, vert3, indices);
            Shape_ shape4 = UnityMessageBuilder.CreateShape_(4, vert3, indices);

            List<Shape_> changedShapes = new List<Shape_>();
            List<Shape_> newShapes = new List<Shape_>();

            changedShapes.Add(shape1);
            changedShapes.Add(shape2);
            newShapes.Add(shape3);
            newShapes.Add(shape4);

            ShapeUpdateInfo_ result = MessageBuilder.CreateShapeUpdateInfo_(changedShapes, newShapes);

            Assert.AreEqual(changedShapes.Count, result.changedShapes.Count);
            Assert.AreEqual(newShapes.Count, result.newShapes.Count);

            for (int i = 0; i < changedShapes.Count; i++)
            {
                Assert.AreEqual(changedShapes[i].id, result.changedShapes[i].id);
                Assert.AreEqual(changedShapes[i].vertices.Count, result.changedShapes[i].vertices.Count);
                Assert.AreEqual(indices.Count, changedShapes[i].indices.Count);
                Assert.AreEqual(0, changedShapes[i].indices[0]);

                for (int j = 0; j < changedShapes[i].vertices.Count; j++)
                {
                    Assert.AreEqual(changedShapes[i].vertices[j].ToUnityVector(),
                        result.changedShapes[i].vertices[j].ToUnityVector());
                }
            }

            for (int i = 0; i < newShapes.Count; i++)
            {
                Assert.AreEqual(newShapes[i].id, result.newShapes[i].id);
                Assert.AreEqual(newShapes[i].vertices.Count, result.newShapes[i].vertices.Count);
                Assert.AreEqual(indices.Count, newShapes[i].indices.Count);
                Assert.AreEqual(0, newShapes[i].indices[0]);

                for (int j = 0; j < newShapes[i].vertices.Count; j++)
                {
                    Assert.AreEqual(newShapes[i].vertices[j].ToUnityVector(),
                        result.newShapes[i].vertices[j].ToUnityVector());
                }
            }
        }

        [Test]
        public static void SetIdentificationResponse()
        {
            Message result = new Message();

            result.SetIdentificationResponse("robot type");

            Assert.IsNotNull(result.identificationResponse);
            Assert.IsNotNull(result.identificationResponse.robotType);

            Assert.AreEqual("robot type", result.identificationResponse.robotType);
        }

        [Test]
        public static void SetLogError()
        {
            Message result = new Message();

            result.SetLogError("error message");

            Assert.IsNotNull(result.error);
            Assert.IsNotNull(result.error.message);

            Assert.AreEqual("error message", result.error.message);
        }

        [Test]
        public static void SetCustomMessage()
        {
            Message result = new Message();

            result.SetCustomMessage("custom key", "custom data");

            Assert.IsNotNull(result.customMessage);
            Assert.IsNotNull(result.customMessage.key);
            Assert.IsNotNull(result.customMessage.data);

            Assert.AreEqual("custom key", result.customMessage.key);
            Assert.AreEqual("custom data", result.customMessage.data);
        }

        [Test]
        public static void SetVelocity()
        {
            Message result = new Message();

            Vector3 linear = new Vector3(1.1f, 2.2f, 3.3f);
            Vector3 angular = new Vector3(4.4f, 5.5f, 6.6f);

            result.SetVelocity(linear, angular);

            Assert.IsNotNull(result.robotVelocity);
            Assert.IsNotNull(result.robotVelocity.linearVelocity);
            Assert.IsNotNull(result.robotVelocity.linearVelocity);

            Assert.AreEqual(linear, result.robotVelocity.linearVelocity.ToUnityVector());
            Assert.AreEqual(angular, result.robotVelocity.angularVelocity.ToUnityVector());
        }

        [Test]
        public static void SetVelocity_nullargs()
        {
            Vector3 linear = new Vector3(1.1f, 2.2f, 3.3f);
            Vector3 angular = new Vector3(4.4f, 5.5f, 6.6f);

            Message result = new Message();
            result.SetVelocity(null, null);

            Assert.IsNotNull(result.robotVelocity);
            Assert.IsNull(result.robotVelocity.linearVelocity);
            Assert.IsNull(result.robotVelocity.angularVelocity);

            result.SetVelocity(linear, null);

            Assert.IsNotNull(result.robotVelocity);
            Assert.IsNotNull(result.robotVelocity.linearVelocity);
            Assert.IsNull(result.robotVelocity.angularVelocity);

            Assert.AreEqual(linear, result.robotVelocity.linearVelocity.ToUnityVector());

            result.SetVelocity(null, angular);

            Assert.IsNotNull(result.robotVelocity);
            Assert.IsNull(result.robotVelocity.linearVelocity);
            Assert.IsNotNull(result.robotVelocity.angularVelocity);

            Assert.AreEqual(angular, result.robotVelocity.angularVelocity.ToUnityVector());
        }

        [Test]
        public static void SetShapeUpdateInfo()
        {
            List<Vector3> vert1 = new List<Vector3>();
            List<Vector3> vert2 = new List<Vector3>();
            List<Vector3> vert3 = new List<Vector3>();
            List<Vector3> vert4 = new List<Vector3>();

            vert1.Add(new Vector3(1, 1, 1));
            vert1.Add(new Vector3(1, 2, 2));
            vert1.Add(new Vector3(1, 3, 3));

            vert2.Add(new Vector3(2, 1, 1));
            vert2.Add(new Vector3(2, 2, 2));
            vert2.Add(new Vector3(2, 3, 3));

            vert3.Add(new Vector3(3, 1, 1));
            vert3.Add(new Vector3(3, 2, 2));
            vert3.Add(new Vector3(3, 3, 3));

            vert4.Add(new Vector3(4, 1, 1));
            vert4.Add(new Vector3(4, 2, 2));
            vert4.Add(new Vector3(4, 3, 3));


            List<UInt32> indices = new List<UInt32>();
            indices.Add(0);
            indices.Add(1);
            indices.Add(2);

            Shape_ shape1 = UnityMessageBuilder.CreateShape_(1, vert1, indices);
            Shape_ shape2 = UnityMessageBuilder.CreateShape_(2, vert2, indices);
            Shape_ shape3 = UnityMessageBuilder.CreateShape_(3, vert3, indices);
            Shape_ shape4 = UnityMessageBuilder.CreateShape_(4, vert3, indices);

            List<Shape_> changedShapes = new List<Shape_>();
            List<Shape_> newShapes = new List<Shape_>();

            changedShapes.Add(shape1);
            changedShapes.Add(shape2);
            newShapes.Add(shape3);
            newShapes.Add(shape4);

            Message result = new Message();
            result.SetShapeUpdateInfo(changedShapes, newShapes);

            Assert.AreEqual(changedShapes.Count, result.shapeUpdateInfo.changedShapes.Count);
            Assert.AreEqual(newShapes.Count, result.shapeUpdateInfo.newShapes.Count);

            for (int i = 0; i < changedShapes.Count; i++)
            {
                Assert.AreEqual(changedShapes[i].id, result.shapeUpdateInfo.changedShapes[i].id);
                Assert.AreEqual(changedShapes[i].vertices.Count, result.shapeUpdateInfo.changedShapes[i].vertices.Count);

                for (int j = 0; j < changedShapes[i].vertices.Count; j++)
                {
                    Assert.AreEqual(changedShapes[i].vertices[j].ToUnityVector(),
                        result.shapeUpdateInfo.changedShapes[i].vertices[j].ToUnityVector());
                }
            }

            for (int i = 0; i < newShapes.Count; i++)
            {
                Assert.AreEqual(newShapes[i].id, result.shapeUpdateInfo.newShapes[i].id);
                Assert.AreEqual(newShapes[i].vertices.Count, result.shapeUpdateInfo.newShapes[i].vertices.Count);

                for (int j = 0; j < newShapes[i].vertices.Count; j++)
                {
                    Assert.AreEqual(newShapes[i].vertices[j].ToUnityVector(),
                        result.shapeUpdateInfo.newShapes[i].vertices[j].ToUnityVector());
                }
            }
        }

        [Test]
        public static void ToUnityVector()
        {
            Vector3_ vec = new Vector3_ { x = 1.1f, y = 2.2f, z = 3.3f };

            Vector3 result = vec.ToUnityVector();

            Assert.AreEqual(result.x, vec.x);
            Assert.AreEqual(result.y, vec.y);
            Assert.AreEqual(result.z, vec.z);
        }

        // Createvector3
    }
}