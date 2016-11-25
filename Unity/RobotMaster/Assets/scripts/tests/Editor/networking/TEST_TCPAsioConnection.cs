using System.Collections.Generic;
using Networking;
using Communication;
using NUnit.Framework;
using System.Net.Sockets;
using System.Threading;
using Communication.Messages;
using Communication.Transform;
using UnityEngine;
using UnityEditor;

namespace NetworkingTests
{

    [TestFixture]
    public class TEST_TCPAsioConnection
    {
        private TestIncomingDataLinkSubscriberCopy _subscriber;
        private TCPDataLinkListener<ProtoBufPresentation> _listener;


        [Test]
        public void AcceptConnection()
        {
            if (EditorPrefs.GetBool("test_tcp_asio") == false)
            {
                Debug.LogWarning("[TEST_TCPAsioConnection.AcceptConnection] Test is disabled, you can enable this test via the menu option \"Testing\"");
                Assert.That(true);
                return;
            }

            Debug.LogWarning("[TEST_TCPAsioConnection.AcceptConnection] This test will only pass when a tcp client is manually connected to the listener started in this test.");

            _subscriber = new TestIncomingDataLinkSubscriberCopy();

            _listener = new TCPDataLinkListener<ProtoBufPresentation>(_subscriber);
            //use public IP address not localhost/127.0.0.1
            //use eduroam(same network)
            try
            {
                Assert.True(_listener.Start("145.93.45.16", 1234));
            }
            catch (SocketException ex)
            {
                Assert.That(false, "Invalid ip address\n" + ex.Message);
            }


            int sleepCount = 0;

            while (!_subscriber.Connected)
            {
                if (sleepCount > 500)
                {
                    Assert.True(false, "Failed to connect within timeout.");
                    break;
                }

                Thread.Sleep(20);

                sleepCount++;
            }
            Debug.Log("Actually connected");
            //Thread.Sleep(8000);

            Assert.IsTrue(_subscriber.Connected);
            Assert.NotNull(_subscriber.DataLink);
            Assert.IsTrue(_subscriber.DataLink.Connected());

            if (_subscriber.Connected)
            {
                Thread.Sleep(3000);
				Communicator comm = new Communicator(_subscriber.DataLink, new ProtoBufPresentation());
				Communication.Message m = new Message
				{
					messageType = Communication.MessageType_.ShapeUpdate,
					messageTarget = Communication.MessageTarget_.Robot,
					id = 5
				};

				Shape_ sh = new Shape_();
				sh.id = 15;
				Vector3_ v = new Vector3_
				{
					x = 3.0f,
					y = 3.0f,
					z = 4.0f
				};

				Vector3_ v1 = new Vector3_
				{
					x = -1.0f,
					y = -3.0f,
					z = -5.5f
				};
				sh.vertices.Add(v);
				sh.vertices.Add(v1);

				Shape_ sh2 = new Shape_();
				sh2.id = 2;
				Vector3_ v2 = new Vector3_
				{
					x = 3.4f,
					y = 3.1f,
					z = 4.7f
				};
				sh2.vertices.Add(v2);

				m.shapeUpdateInfo = new ShapeUpdateInfo_();
				//m.shapeUpdateInfo.changedShapes = new List<Shape_>();
				m.shapeUpdateInfo.changedShapes.Add(sh);
				m.shapeUpdateInfo.changedShapes.Add(sh2);
				m.customMessage = new CustomMessage_();
				m.customMessage.key = "asdf";
				m.customMessage.data = "asdf data";

				Assert.IsTrue(comm.SendCommand(m));
            }
        }

        [TearDown]
        public void Cleanup()
        {
            //  _incomingClient.Close();
            if (_subscriber != null)
            {
                _subscriber.Cleanup();
            }
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }

    public class TestIncomingDataLinkSubscriberCopy : IIncomingDataLinkSubscriber, IMessageReceiver
    {
        public volatile bool Connected = false;

        public volatile IDataLink DataLink;

        public void IncomingNewDataLink(IDataLink dataLink, IPresentationProtocol usedProtocol)
        {
            this.DataLink = dataLink;
            usedProtocol.SetReceiver(this);
            Connected = true;
            Debug.Log("Incoming impl");
        }

        public void IncomingMessage(Message newMessage, IDataLink datalink)
        {
            Debug.Log(newMessage.stringData);
			foreach (var shape in newMessage.shapeUpdateInfo.changedShapes)
			{
				Debug.Log(shape.id);
				foreach (var vertex in shape.vertices)
				{
					Debug.Log(string.Format("x:{0}, y:{1}, z:{2}", vertex.x, vertex.y, vertex.z));
				}

			}
        }

        public void Cleanup()
        {
            if (this.DataLink != null)
            {
                this.DataLink.Dispose();
            }
        }
    }

    // Menu item for toggeling this test.
    public class Tcp_Asio_Test_Toggle : EditorWindow
    {

        [MenuItem("Testing/Enable tcp_asio test")]
        static void Enable_Tcp_Asio_Test()
        {
            EditorPrefs.SetBool("test_tcp_asio", true);

            EditorUtility.DisplayDialog(
                "Tcp-Asio test enable",
                "Tcp-Asio tests are now enabled.",
                "OK");
        }

		[MenuItem("Testing/Disable tcp_asio test")]
		static void Disable_Tcp_Asio_Test()
		{
			EditorPrefs.SetBool("test_tcp_asio", false);

			EditorUtility.DisplayDialog(
				"Tcp-Asio test disable",
				"Tcp-Asio tests are now disabled.",
				"OK");
		}

		[MenuItem("Testing/Enable tcp_asio test", true)]
		static bool Enable_Tcp_Asio_Test_Validate()
		{
			return EditorPrefs.GetBool("test_tcp_asio") == false;
		}

		[MenuItem("Testing/Disable tcp_asio test", true)]
		static bool Disable_Tcp_Asio_Test_Validate()
		{
			return EditorPrefs.GetBool("test_tcp_asio") == true;
		}
    }
}
