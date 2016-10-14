using Networking;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace NetworkingTests
{

    public class unnitTest_DataReceiver : IDataStreamReceiver
    {
        public byte[] expectedResult;
        public volatile bool receivedData = false;
        public volatile bool failed = false;


        public void IncomingData(byte[] data, IDataLink datalink)
        {
            receivedData = true;

            failed = !(Encoding.ASCII.GetString(data).CompareTo(
                Encoding.ASCII.GetString(expectedResult)) == 0);
        }
    }


    [TestFixture]
    public class UnitTest_TCPDataLink
    {
        private Thread _helperThread;
        private TCPDataLink test;

        [Test]
        public void SendingAndReceiving()
        {
            TcpClient clientSender;
            TcpClient clientReader;

            SpawnSockets(out clientSender, out clientReader);

            unnitTest_DataReceiver receiver = new unnitTest_DataReceiver();

            test = new TCPDataLink(clientReader);
            test.SetReceiver(receiver);

            receiver.expectedResult = Encoding.ASCII.GetBytes("Hello there");

            clientSender.GetStream().Write(receiver.expectedResult, 0, receiver.expectedResult.Length);

            while (!receiver.receivedData) { Thread.Sleep(1); }

            Assert.IsFalse(receiver.failed, "Expected result is different from actual result");



            receiver.expectedResult = Encoding.ASCII.GetBytes("Goodbye pal!");

            clientSender.GetStream().Write(receiver.expectedResult, 0, receiver.expectedResult.Length);

            while (!receiver.receivedData) { Thread.Sleep(1); }

            Assert.IsFalse(receiver.failed, "Expected result is different from actual result");

        }

        private TcpClient helperTcpClient;
        private string IP;
        private int port;


        /// <summary>
        /// SpawnSockets uses a thread in order to spawn internally connected sockets (TcpClient).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private void SpawnSockets(out TcpClient sender, out TcpClient reader)
        {
            TcpListener server = null;

            IP = "127.0.0.1";

            sender = null;
            reader = null;

            IPAddress localAddr = IPAddress.Parse(IP);

            // TcpListener server = new TcpListener(port);
            port = 1234;
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            Debug.Log("[TEST] Waiting for a connection... ");

            _helperThread = new Thread(ConnectSender);

            helperTcpClient = new TcpClient();

            _helperThread.Start(this);


            // Perform a blocking call to accept requests.
            // You could also user server.AcceptSocket() here.
            reader = server.AcceptTcpClient();
            Debug.Log("[TEST] Connection found!");

            sender = helperTcpClient;

            _helperThread.Join();

            server.Stop();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            _helperThread.Join();

            if (test != null)
                test.Dispose();
        }

        static void ConnectSender(object _tester)
        {
            UnitTest_TCPDataLink tester = (UnitTest_TCPDataLink)_tester;

            tester.helperTcpClient.Connect(tester.IP, tester.port);
        }
    }
}