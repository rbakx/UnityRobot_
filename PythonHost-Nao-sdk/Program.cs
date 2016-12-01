using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PhysicalControllers;

partial class Program
{
    static NaoBroker robot = null;

    static partial void main(string[] arguments)
    {
        if(arguments.Length >= 2)
        { 
            string ip = arguments[1];
            short port = 9559;

            if(short.TryParse(arguments[2], out port))
                {  if (port < 1) { port = 9559; } }

            robot = new NaoBroker(ip, port);

            Console.WriteLine("Nao connected with ID " + robot.GetId());

            robot.Motion_WakeUp();

            //

            XboxController controller = new XboxController(0);

            controller.OnButtonChange += OnButtonUpdate;

            Console.WriteLine("Spawned controller on Id 0");

            Thread.Sleep(10000000);
        }
    }
}