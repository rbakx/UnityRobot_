using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace broker
{
    public class RobotBroker : IDisposable
    {
        private bool _connected;

        private GeneralTypeRobot _robot;

        private IPresentationProtocol _protocol;
        private IDataLink _dataLink;
        private Communicator _communicator;

        private bool _disposed;

        public Communicator Communicator { get { return _communicator; } }

        public RobotBroker()
        {
            _connected = false;

            _robot = null;

            _protocol = null;
            _dataLink = null;
            _communicator = null;

            _disposed = false;
        }

        public bool Connect(string unityIp, ushort unityPort, int timeout = -1)
        {
            //Creates a connection to the robot using the specified hostname and port
            TcpClient tcpClient = new TcpClient();
            IAsyncResult ar = tcpClient.BeginConnect(unityIp, unityPort, null, null);
            WaitHandle wh = ar.AsyncWaitHandle;
            try
            {
                if (!wh.WaitOne(timeout, false))
                {
                    tcpClient.Close();
                    wh.Close();
                    return false;
                }

                tcpClient.EndConnect(ar);

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                wh.Close();
            }

            _dataLink = new TCPDataLink(tcpClient);
            _protocol = new ProtoBufPresentation();
            _dataLink.SetReceiver(_protocol);

            _communicator = new Communicator(_dataLink, _protocol);

            _connected = true;
            return true;
        }

        public bool ConnectRobot(GeneralTypeRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentNullException("robot");
            }

            if (!_connected)
            {
                throw new InvalidOperationException("Can't connect robot without being connected");
            }

            _robot = robot;
            MessageActionMapper map = new MessageActionMapper(_robot);
            _protocol.SetReceiver(map);

            return true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_connected && _dataLink != null)
                {
                    _dataLink.Dispose();
                }

                _disposed = true;
                _connected = false;
            }
        }
    }
}
