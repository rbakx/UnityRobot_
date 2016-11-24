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
        private const byte EV3_RESPONSE_LOCALS_GLOBALS = 0xfb;  // 251 global variables, for a response of 256 bytes

        private string _serialNumber;
        private string _projectName;
        private string _lastMessage;

        private short _tcpPort;
        private TCPDataLink _tcpDataLink;
        private DataStreamReceiver _dataStreamReceiver;
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

            _serialNumber = null;
            _projectName = projectName;
            _lastMessage = null;

            _tcpPort = -1;
            _tcpDataLink = null;
            _dataStreamReceiver = new DataStreamReceiver(OnIncomingData);
            _receiveSem = new Semaphore(0, 1);
        }

        public bool Connect(int timeout = -1)
        {
            IPEndPoint ev3Endpoint = NegotiateUdp(timeout);
            if (ev3Endpoint != null)
            {
                return StartTcp(ev3Endpoint, timeout);
            }

            return false;
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
                msgData[0] = (byte)(msgLen - 2);                       //  Message length (excluding 2 length bytes)
                msgData[1] = 0x0;                                      //  Message length
                msgData[2] = 0x0;                                      //  Message counter (unused)
                msgData[3] = 0x0;                                      //  Message counter (unused)
                msgData[4] = (byte)EV3Command.SYSTEM_COMMAND_NO_REPLY; //  Command type
                msgData[5] = (byte)EV3Command.WRITEMAILBOX;            //  System command
                msgData[6] = (byte)(mailBox.Length + 1);               //  Mailbox name length (including null terminator)

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

        public string ReceiveString(string mailBox, int timeout = -1)
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

                msgData[0] = (byte)(msgLen - 2);                     //  Message length (excluding 2 length bytes)
                msgData[1] = 0x0;                                    //  Message length
                msgData[2] = 0x0;                                    //  Message counter (unused)
                msgData[3] = 0x0;                                    //  Message counter (unused)
                msgData[4] = (byte)EV3Command.DIRECT_COMMAND_REPLY;  //  Command type
                msgData[5] = EV3_RESPONSE_LOCALS_GLOBALS;            //  Number of globals and locals reserved for response	
                msgData[6] = 0x0;                                    //  Number of globals and locals
                msgData[7] = (byte)EV3Command.OPFILE;                //  Command
                msgData[8] = (byte)EV3Command.OPFILE_OPEN_READ;      //  Subcommand
                msgData[9] = (byte)EV3Command.LOCAL_CONSTANT_STRING; //  Indicate a string will follow

                // [10..n] filename string
                Array.Copy(Encoding.ASCII.GetBytes(fileName), 0, msgData, 10, fileName.Length);
                msgData[10 + fileName.Length] = (byte)'\0';

                int n = 10 + fileName.Length;

                msgData[n + 1] = 0x60;                              // Returned file handle offset: 0
                msgData[n + 2] = 0x64;                              // Returned file size offset: 4
                msgData[n + 3] = (byte)EV3Command.OPFILE;           // Next command in stream
                msgData[n + 4] = (byte)EV3Command.OPFILE_READ_TEXT; // Subcommand
                msgData[n + 5] = 0x60;                              // File handle location
                msgData[n + 6] = 0x00;                              // Delimiter code: no delimiter
                msgData[n + 7] = 0xf0;                              // Max string length to read: 240
                msgData[n + 8] = 0x68;                              // Returned string offset: 8
                msgData[n + 9] = (byte)EV3Command.OPFILE;           // Next command 
                msgData[n + 10] = (byte)EV3Command.OPFILE_CLOSE;    // Subcommand
                msgData[n + 11] = 0x60;                             // File handle location

                _tcpDataLink.SendData(msgData);
                _receiveSem.WaitOne(timeout);

                return _lastMessage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        private IPEndPoint NegotiateUdp(int timeout = -1)
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
                bool success = short.TryParse(tcpPortStr, out _tcpPort);

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

        private bool StartTcp(IPEndPoint ev3Endpoint, int timeout = -1)
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
                _receiveSem.WaitOne(timeout);

                _dataStreamReceiver.SetCallback(OnIncomingTcpData);

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

        private void OnIncomingTcpData(byte[] data)
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
