using broker;
using Networking;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ev3_broker
{
    public class EV3ServiceListener : RobotServiceListener
    {
        private ushort _broadcastPort;
        private UdpClient _udpClient;

        IPAddress _unityIP;
        private ushort _unityPort;

        private List<string> _connectedSerials;

        private const ushort EV3_UDP_BROADCAST_PORT = 3015;
        private const string EV3_UDP_RESPONSE_MSG = "hi";

        public EV3ServiceListener(IRobotServiceEventsReceiver eventReceiver,
            IPAddress unityIP,
            ushort unityPort,
            ushort broadcastPort = EV3_UDP_BROADCAST_PORT)
            : base(eventReceiver)
        {
            _broadcastPort = broadcastPort;

            _udpClient = null;

            if (unityIP == null)
            {
                throw new ArgumentNullException("unityIP");
            }

            _unityIP = unityIP;
            _unityPort = unityPort;

            _connectedSerials = new List<string>();
        }

        public override bool Connected()
        {
            throw new NotImplementedException();
        }

        public override string GetServiceName()
        {
            throw new NotImplementedException();
        }

        public override bool StartListening()
        {
            try
            {
                _udpClient = new UdpClient(_broadcastPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            Task.Run(() => _OnReceive());

            return true;
        }

        public override void StopListening()
        {
            _udpClient.Close();
            _udpClient = null;
        }

        private async void _OnReceive()
        { 
            while (_udpClient != null)
            {
                UdpReceiveResult results;
                try
                {
                    results = await _udpClient.ReceiveAsync();
                }
                catch (ObjectDisposedException)
                {
                    continue;
                }

                string msgStr = Encoding.ASCII.GetString(results.Buffer);
                Regex serialRegex = new Regex("Serial-Number: (.*)");
                Regex portRegex = new Regex("Port: (.*)");

                string serialNumber = null;
                int port = -1;

                Match match = serialRegex.Match(msgStr);
                if (match.Success)
                {
                    serialNumber = match.Groups[1].Value;
                }
                /* serialNumber != null && */
                if ( !_connectedSerials.Contains(serialNumber))
                {
                    match = portRegex.Match(msgStr);
                    if (match.Success)
                    {
                        string tcpPortStr = match.Groups[1].Value.TrimEnd();
                        bool success = int.TryParse(tcpPortStr, out port);

                        if (!success)
                        {
                            port = -1;
                        }
                    }

                    _connectedSerials.Add(serialNumber);
                    SendUdpResponse(_udpClient, results.RemoteEndPoint);
                    TCPDataLink dl = EstablishTcpConnection(serialNumber,
                        results.RemoteEndPoint.Address, port);

                    Ev3Connection con = new Ev3Connection("EV3Wifi", dl);

                    Communicator communicator = null;

                    if (Tools.ConnectToUnity(out communicator, _unityIP.ToString(), _unityPort,
                        5000))
                    {
                        Console.WriteLine("Unity connected");

                        EV3Robot newRobot = new EV3Robot(communicator, "My ev3", con);
                        Tools.AssignRobot(communicator, newRobot);

                        _eventReceiver.OnRobotConnect(newRobot);
                    }
                    else
                    {
                        throw new Exception("Can't connect to unity...");
                    }
                }
            }
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

        private TCPDataLink EstablishTcpConnection(string serial, IPAddress address, int tcpPort)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (tcpPort < 1)
            {
                throw new Exception("_tcpPort not set");
            }

            TcpClient tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect(new IPEndPoint(address, tcpPort));

                string str = "GET /target?sn=" + serial + " VMTP1.0\nProtocol: EV3";
                byte[] msgBuf = Encoding.ASCII.GetBytes(str);
                tcpClient.GetStream().Write(msgBuf, 0, msgBuf.Length);

                short timeout = 5;
                while (!tcpClient.GetStream().CanRead)
                {
                    Thread.Sleep(1);
                    if (timeout-- <= 0)
                    {
                        break;
                    }
                }

                byte[] receiveBuffer = new byte[16];
                int read = tcpClient.GetStream().Read(receiveBuffer, 0, receiveBuffer.Length);
                string response = Encoding.ASCII.GetString(receiveBuffer);
                response =  response.TrimEnd();

                if (response != null)
                {
                    Console.WriteLine("TCP Test response: " + response);
                }

                return new TCPDataLink(tcpClient);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                tcpClient.Close();
                return null;
            }
        }
    }
}
