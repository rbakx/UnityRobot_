using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Networking;
using broker;
using nao_broker;
using System.Reflection;

partial class Program
{
    static partial void main(string[] arguments)
    {
        if (arguments.Length >= 4)
        {
            string nao_ip = arguments[1];
            short nao_port = 9559;

            if (short.TryParse(arguments[2], out nao_port))
            { if (nao_port < 1) { nao_port = 9559; } }

            string unity_ip = arguments[3];
            ushort unity_port = 9559;

            if (ushort.TryParse(arguments[4], out unity_port))
            { if (unity_port < 1) { unity_port = 9559; } }

            //Throws exception when fails to connect
            NaoConnection naoCon = new NaoConnection(nao_ip, nao_port);

            Console.WriteLine("Nao connected");

            Communicator communicator = null;

            if(Tools.ConnectToUnity(out communicator, unity_ip, unity_port, 5000))
            {
                using(NaoRobot robot = new NaoRobot(communicator, "My little nao", naoCon))
                {
                    Tools.AssignRobot(communicator, robot);

                    Console.WriteLine("Nao connected with ID " + naoCon.GetId());

                    naoCon.Motion_WakeUp();

                    Thread.Sleep(10000000);
                }
            }
            else
            {
                Console.WriteLine("Failed to connect with unity");
            }
        }
    }
}