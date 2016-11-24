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
            DIRECT_COMMAND_REPLY = 0x00,

            OPFILE = 0xc0,
            OPFILE_OPEN_READ = 0x01,
            OPFILE_READ_TEXT = 0x05,
            OPFILE_CLOSE = 0x07,

            SYSTEM_COMMAND_NO_REPLY = 0x81,

            WRITEMAILBOX = 0x9e,

            LOCAL_CONSTANT_STRING = 0x84,
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

            public void SetCallback(OnIncomingData incomingDataCallback)
            {
                if (incomingDataCallback != null)
                {
                    _incomingDataCallback = incomingDataCallback;
                }
            }

            public void IncomingData(byte[] data, IDataLink datalink)
            {
                _incomingDataCallback(data);
            }
        }

        private const short EV3_UDP_BROADCAST_PORT = 3015;
        private const string EV3_UDP_RESPONSE_MSG = "hi";
        private const byte EV3_RESPONSE_LOCALS_GLOBALS = 0xfb;
        // 251 global variables, for a response of 256 bytes

        private string _serialNumber;
        private int _tcpPort;
        private string _projectName;
        private string _lastMessage;
        private TCPDataLink _tcpDataLink;
        private Semaphore _receiveSem;

        private DataStreamReceiver _dataStreamReceiver;

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
            _dataStreamReceiver = new DataStreamReceiver(OnIncomingData);

            _tcpDataLink = null;
            _tcpPort = -1;

            _lastMessage = null;
            _serialNumber = null;
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
                    new byte[] {
                        (byte)(msgLen - 2),                       // [0] Message length (excluding 2 length bytes)
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
        }

        // NOTE: Max reply size is 256 bytes
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

            string fileName = string.Format("../prjs/{0}/{1}.rtf", _projectName, mailBox);

            try
            {
                byte[] msgData;
                int msgLen = 22 + fileName.Length;
                msgData = new byte[msgLen];

                Array.Copy(
                    new byte[] {
                        (byte)(msgLen - 2),                     // [0] Message length (excluding 2 length bytes)
						0x0,			                        // [1] Message length
						0x0,                                    // [2] Message counter (unused)
						0x0,                                    // [3] Message counter (unused)
						(byte)EV3Command.DIRECT_COMMAND_REPLY,  // [4] Command type
						EV3_RESPONSE_LOCALS_GLOBALS,            // [5] Number of globals and locals reserved for response	
						0x0,                                    // [6] Number of gloabls and locals
						(byte)EV3Command.OPFILE,                // [7] Command
						(byte)EV3Command.OPFILE_OPEN_READ, 	    // [8] Subcommand
						(byte)EV3Command.LOCAL_CONSTANT_STRING, // [9] Indicate a string will follow
					},
                    msgData,
                    10
                );

                // [10..n] filename string
                Array.Copy(Encoding.ASCII.GetBytes(fileName), 0, msgData, 10, fileName.Length);

                // [n+1] // Zero terminator for filename string
                msgData[10 + fileName.Length] = (byte)'\0';

                Array.Copy(
                    new byte[]
                    {
                        0x60,                              // [n+2] Returned file handle offset: 0
        				0x64,                              // [n+3] Returned file size offset: 4
        				(byte)EV3Command.OPFILE,           // [n+4] Next command in stream
        				(byte)EV3Command.OPFILE_READ_TEXT, // [n+5] Subcommand
        				0x60,                              // [n+6] File handle location
        				0x00,                              // [n+7] Delimiter code: no delimiter
        				0xf0,                              // [n+8] Max string length to read: 240
        				0x68,                              // [n+9] Returned string offset: 8
        				(byte)EV3Command.OPFILE,           // [n+10] Next command 
        				(byte)EV3Command.OPFILE_CLOSE,     // [n+11] Subcommand
        				0x60,                              // [n+12] File handle location
                    },
                    0,
                    msgData,
                    11 + fileName.Length,
                    11
                );

                _tcpDataLink.SendData(msgData);
                _receiveSem.WaitOne();

                return _lastMessage.TrimEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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
            Regex serialRegex = new Regex("Serial-Number: (.*)");
            Regex portRegex = new Regex("Port: (.*)");

            Match match = serialRegex.Match(msgStr);
            if (match.Success)
            {
                _serialNumber = match.Groups[1].Value;
            }
            else
            {
                return false;
            }

            match = portRegex.Match(msgStr);
            if (match.Success)
            {
                string tcpPortStr = match.Groups[1].Value.TrimEnd();
                bool success = int.TryParse(tcpPortStr, out _tcpPort);

                if (!success)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


            return true;
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

            if (_tcpPort < 1)
            {
                throw new Exception("_tcpPort not set");
            }

            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(new IPEndPoint(ev3Endpoint.Address, _tcpPort));

                _tcpDataLink = new TCPDataLink(tcpClient);
                _tcpDataLink.SetReceiver(_dataStreamReceiver);

                string str = "GET /target?sn=" + _serialNumber + " VMTP1.0\nProtocol: EV3";
                _tcpDataLink.SendData(str);
                _receiveSem.WaitOne();
                
                _dataStreamReceiver.SetCallback(OnIncomingTCPData);

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

        private void OnIncomingTCPData(byte[] data)
        {
            if (data.Length > 0)
            {
                if (data.Length == 256)
                {
                    // Messagebox on EV3 was not found
                    if (data[9] == 0)
                    {
                        throw new Exception("Messagebox not found...");
                    }
                    else
                    {
                        _lastMessage = Encoding.ASCII.GetString(data, 13, data[9] - 1);
                    }
                }
                else
                {
                    _lastMessage = Encoding.ASCII.GetString(data);
                }

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
