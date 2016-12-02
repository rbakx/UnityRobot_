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
            string hostname = args.Length > 0 ? args[0] : "192.168.0.100";
            ushort port = (ushort)(args.Length > 1 ? Int32.Parse(args[1]) : 1234);

            Ev3Connection ev3Con = new Ev3Connection("EV3Wifi");
            if (false || !ev3Con.Connect(5000))
            {
                Console.WriteLine("Failed to connect with EV3");
            }
            else
            {
                Console.WriteLine("Ev3 connected");

                using (RobotBroker broker = new RobotBroker())
                {
                    if (broker.Connect(hostname, port, 5000))
                    {
                        Console.WriteLine("Unity connected");
                        using (EV3Robot ev3 = new EV3Robot(broker.Communicator, "My little robot", ev3Con))
                        {
                            broker.ConnectRobot(ev3);

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
            }
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}
