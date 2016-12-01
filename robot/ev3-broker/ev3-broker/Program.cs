using broker;
using Networking;
using System;
using System.Net.Sockets;

namespace ev3_broker
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (EV3Robot ev3 = new EV3Robot(null, "ev3 robot"))
            //{
            //while (true)
            //{
            //    ev3.Indicate();
            //    Console.ReadLine();
            //}


            //ev3.VelocitySet(MessageBuilder.CreateVector(20, 0, 0), MessageBuilder.CreateVector(0, 0, 0));
            //Console.ReadLine();
            ////ev3.VelocitySet(MessageBuilder.CreateVector(0, 0, 0), MessageBuilder.CreateVector(0, 0, -20));
            //Console.ReadLine();
            //ev3.VelocitySet(MessageBuilder.CreateVector(0, 0, 0), MessageBuilder.CreateVector(0, 0, 0));

            //}

            //Gets the hostname and port from the command line parameters to connect to the robot in question (in this case the Nao)
            string hostname = args.Length > 0 ? args[0] : "192.168.0.102";
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
                Console.ReadLine();
                return;
            }

            Ev3Connection ev3Con = new Ev3Connection("EV3Wifi");
            if (!ev3Con.Connect(5000))
            {
                throw new Exception("EV3 not connected");
            }
            Console.WriteLine("Ev3 connected");

            IDataLink dl = new TCPDataLink(tcpClient);
            IPresentationProtocol pp = new ProtoBufPresentation();
            dl.SetReceiver(pp);

            Communicator communicator = new Communicator(dl, pp);

            EV3Robot ev3 = new EV3Robot(communicator, "My little robot", ev3Con);
            MessageActionMapper map = new MessageActionMapper(ev3);
            pp.SetReceiver(map);

            //Waiting for user input before closing
            Console.WriteLine("Press enter to close the connection.");
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                    break;
            }

            //Clean up
            ev3.Dispose();
            dl.Dispose();
            tcpClient.Close();

            Console.ReadLine();
        }
    }
}
