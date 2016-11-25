using broker;
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace nao_broker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Gets the hostname and port from the command line parameters to connect to the robot in question (in this case the Nao)
            string hostname = args.Length > 0 ? args[0] : "127.0.0.1";
            int port = args.Length > 1 ? Int32.Parse(args[1]) : 1234;

            //Creates a connection to the robot using the specified hostname and port
            TcpClient tcpClient;
            try
            {
                tcpClient = new TcpClient(hostname, port);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            IDataLink dl = new TCPDataLink(tcpClient);
            IPresentationProtocol pp = new ProtoBufPresentation();

            Communicator communicator = new Communicator(dl, pp);

            NaoRobot nao = new NaoRobot(communicator, "My little robot");
            MessageActionMapper map = new MessageActionMapper(nao);
            pp.SetReceiver(map);


            //Waiting for user input before closing
            Console.WriteLine("Press enter to close the connection.");
            while(true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                    break;
            }

            //Clean up
            tcpClient.Close();
        }
    }
}
