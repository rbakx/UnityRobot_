using broker;
using Communication;
using Networking;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ev3_broker
{
    class Program
    {
        class Receiver : IRobotServiceEventsReceiver, IDisposable
        {
            private List<EV3Robot> _robots;

            public List<EV3Robot> robots {  get { return _robots; } }

            public Receiver()
            {
                _robots = new List<EV3Robot>();
            }
            
            public void OnRobotConnect(GeneralTypeRobot robot)
            {
                Console.WriteLine("robot connected");
                _robots.Add(robot as EV3Robot);
            }

            public void Dispose()
            {
                foreach (var rob in _robots)
                {
                    rob.Dispose();
                }
            }
        }
        static void Main(string[] args)
        {

           Receiver receiver = new Receiver();
            EV3ServiceListener listener = new EV3ServiceListener(receiver,
                IPAddress.Parse("192.168.0.101"), 1234);
            listener.StartListening();

            Console.WriteLine("Listener started, press enter to quit");
            Console.ReadLine();


            foreach (EV3Robot robot in receiver.robots)
            {
                robot.Disconnect();
            }

            listener.StopListening();
            receiver.Dispose();
            Console.WriteLine("Exiting program.");
        }
    }
}
