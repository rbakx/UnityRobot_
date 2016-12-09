using broker;
using Communication;
using Networking;
using System;
using System.Net.Sockets;

namespace ev3_broker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Gets the hostname and port from the command line parameters to connect to the robot in question(in this case the Nao)
            string hostname = args.Length > 0 ? args[0] : "192.168.0.101";
            ushort port = (ushort)(args.Length > 1 ? Int32.Parse(args[1]) : 1234);

            Ev3Connection ev3Con = new Ev3Connection("EV3Wifi");
            if (!ev3Con.Connect(5000))
            {
                Console.WriteLine("Failed to connect with EV3");
                ev3Con.Dispose();
            }
            else
            {
                Console.WriteLine("Ev3 connected");

                Communicator communicator = null;

                if (Tools.ConnectToUnity(out communicator, hostname, port, 5000))
                {
                    Console.WriteLine("Unity connected");

                    using (EV3Robot ev3 = new EV3Robot(communicator, "My little robot", ev3Con))
                    {
                        Tools.AssignRobot(communicator, ev3);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect with unity");
                    ev3Con.Dispose();
                }
            }
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}
