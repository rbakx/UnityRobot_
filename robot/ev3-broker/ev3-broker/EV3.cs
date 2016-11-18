using Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ev3_broker
{
    class EV3 : IDisposable
    {
        private enum EV3Command : byte
        {
            SYSTEM_COMMAND_REPLY = 0x01,
            SYSTEM_COMMAND_NO_REPLY = 0x81,

            WRITEMAILBOX = 0x9e,
        }

        /// <summary>
        /// The DataStreamReceiver is used so that the actual callback in the EV3 
        /// class can be private.
        /// </summary>
        private class DataStreamReceiver : IDataStreamReceiver
        {
            public delegate void OnIncomingData(byte[] data);

            private OnIncomingData _incomingDataCallback;

            public DataStreamReceiver(OnIncomingData incomingDataCallback)
            {
                if (incomingDataCallback == null)
                {
                    throw new ArgumentNullException("incomingDataCallback");
                }

                _incomingDataCallback = incomingDataCallback;
            }

            public void IncomingData(byte[] data, IDataLink datalink)
            {
                _incomingDataCallback(data);
            }
        }

        private const short EV3_UDP_BROADCAST_PORT = 3015;
        private const string EV3_UDP_RESPONSE_MSG = "hi";

        private string _serialNumber;
        private string _projectName;
        private string _lastMessage;
        private TCPDataLink _tcpDataLink;
        private Semaphore _receiveSem;

        private bool _disposed = false;

        public string SerialNumber { get { return _serialNumber; } }

        public EV3(string projectName)
        {
            if (projectName == null)
            {
                throw new ArgumentNullException("projectName");
            }
            if (projectName.Length < 1)
            {
                throw new ArgumentException("projectName can't be an empty string", "projectName");
            }

            _projectName = projectName;
            _receiveSem = new Semaphore(0, 1);
        }

        public bool Connect(int timeout = 5000)
        {
            IPEndPoint ev3Endpoint = NegotiateUdp(timeout);
            if (ev3Endpoint != null)
            {
                return StartTcp(ev3Endpoint);
            }
            else
            {
                return false;
            }
        }

        public bool SendMessage(string mailBox, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (message.Length < 1)
            {
                throw new ArgumentException("message can't be an empty string", "message");
            }

            byte[] messageData = new byte[message.Length + 1];
            Array.Copy(Encoding.ASCII.GetBytes(message), messageData, message.Length);
            messageData[messageData.Length - 1] = (byte)'\0';

            return SendMessage(mailBox, messageData);
        }

        public bool SendMessage(string mailBox, float message)
        {
            byte[] messageData = BitConverter.GetBytes(message);
            return SendMessage(mailBox, messageData);
        }
        
        public bool SendMessage(string mailBox, byte[] messageData)
        {
            if (mailBox == null)
            {
                throw new ArgumentNullException("mailBox");
            }
            if (mailBox.Length < 1)
            {
                throw new ArgumentException("mailBox can't be an empty string", "mailBox");
            }

            if (messageData == null)
            {
                throw new ArgumentNullException("messageData");
            }

            try
            {
                byte[] msgData;
                int msgLen = 10 + mailBox.Length + messageData.Length;
                msgData = new byte[msgLen];

                // Message header
                Array.Copy(
                    new byte[]
                    {
                        (byte)(msgLen-2),                         // [0] Message length (excluding 2 length bytes)
                        0x0,                                      // [1] Message length
                        0x0,                                      // [2] Message counter (unused)
                        0x0,                                      // [3] Message counter (unused)
                        (byte)EV3Command.SYSTEM_COMMAND_NO_REPLY, // [4] Command type
                        (byte)EV3Command.WRITEMAILBOX,            // [5] System command
                        (byte)(mailBox.Length + 1),               // [6] Mailbox name length (including null terminator)
                    },
                    msgData,
                    7);

                // [7..n] Mailbox name
                Array.Copy(Encoding.ASCII.GetBytes(mailBox), 0, msgData, 7, mailBox.Length);
                // [n + 1] null terminator for mailbox name
                msgData[7 + mailBox.Length] = (byte)'\0';

                // [n+2..n+3] Message length (including null terminator)
                msgData[7 + mailBox.Length + 1] = (byte)(msgData.Length + 1);
                // [n+4..n+m] Message
                Array.Copy(messageData, 0, msgData, 7 + mailBox.Length + 3, messageData.Length);

                return _tcpDataLink.SendData(msgData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return false;
        }

        public string ReceiveMessage(string mailBox)
        {
            if (mailBox == null)
            {
                throw new ArgumentNullException("mailBox");
            }
            if (mailBox.Length < 1)
            {
                throw new ArgumentException("Mailbox can't be an empty string", "mailBox");
            }

            _receiveSem.WaitOne();
            
            return null;
        }

        private IPEndPoint NegotiateUdp(int timeout)
        {
            using (UdpClient udpClient = new UdpClient(EV3_UDP_BROADCAST_PORT))
            {
                try
                {
                    udpClient.Client.ReceiveTimeout = timeout;

                    IPEndPoint ev3Endpoint = null;
                    byte[] msgData = udpClient.Receive(ref ev3Endpoint);

                    if (ParseUdpBroadcast(msgData) &&
                        SendUdpResponse(udpClient, ev3Endpoint))
                    {
                        return ev3Endpoint;
                    }
                }
                catch (SocketException)
                {
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return null;
        }

        // TODO: receive the tcp port from msgStr here
        private bool ParseUdpBroadcast(byte[] udpData)
        {
            if (udpData == null)
            {
                throw new ArgumentNullException("udpData");
            }

            string msgStr = Encoding.ASCII.GetString(udpData);
            Regex regex = new Regex("Serial-Number: (.*)");
            Match match = regex.Match(msgStr);

            if (match.Success)
            {
                _serialNumber = match.Groups[1].Value;
                return true;
            }

            return false;
        }

        private bool SendUdpResponse(UdpClient udpClient, IPEndPoint destEndpoint)
        {
            if (udpClient == null)
            {
                throw new ArgumentNullException("udpClient");
            }

            if (destEndpoint == null)
            {
                throw new ArgumentNullException("destEndpoint");
            }

            byte[] respData = Encoding.ASCII.GetBytes(EV3_UDP_RESPONSE_MSG);

            try
            {
                udpClient.Send(respData, respData.Length, destEndpoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private bool StartTcp(IPEndPoint ev3Endpoint)
        {
            if (ev3Endpoint == null)
            {
                throw new ArgumentNullException("ev3Endpoint");
            }

            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(new IPEndPoint(ev3Endpoint.Address, 5555));

                _tcpDataLink = new TCPDataLink(tcpClient);
                _tcpDataLink.SetReceiver(new DataStreamReceiver(OnIncomingData));

                string str = "GET /target?sn=" + _serialNumber + " VMTP1.0\nProtocol: EV3";
                _tcpDataLink.SendData(str);
                _receiveSem.WaitOne();

                Console.WriteLine("TCP Test response: " + _lastMessage.TrimEnd());
            }
            catch (SocketException)
            {
                return false;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private void OnIncomingData(byte[] data)
        {
            if (data.Length > 0)
            {
                _lastMessage = Encoding.ASCII.GetString(data);
                _receiveSem.Release();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _tcpDataLink.Dispose();
            }
        }
    }
}
