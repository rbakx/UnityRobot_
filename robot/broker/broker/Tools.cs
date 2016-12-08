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
    public class Tools
    {
        public static bool ConnectToUnity(out Communicator communicator, string unityIp, ushort unityPort, int timeout = -1)
        {
            communicator = null;

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

            IDataLink _dataLink = new TCPDataLink(tcpClient);
            IPresentationProtocol _protocol = new ProtoBufPresentation();
            _dataLink.SetReceiver(_protocol);

            communicator = new Communicator(_dataLink, _protocol);

            return true;
        }

        public static bool AssignRobot(Communicator communicator, GeneralTypeRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentNullException("robot");
            }

            if(communicator == null)
            {
                throw new ArgumentNullException("communicator");
            }

            IPresentationProtocol pp = communicator.GetPresentationProtocol();

            if(pp == null)
            {
                throw new ArgumentNullException("communicator.GetPresentationProtocol()");
            }

            IDataLink dl = communicator.GetDataLink();

            if (dl == null || communicator.GetDataLink().Connected() != true)
            {
                throw new InvalidOperationException("Can't assign robot without being connected");
            }

            MessageActionMapper map = new MessageActionMapper(robot);
            communicator.GetPresentationProtocol().SetReceiver(map);

            return true;
        }
    }
}
