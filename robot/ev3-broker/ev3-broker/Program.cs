using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EV3WifiLib;
using System.Threading;

namespace ev3_broker
{
    class Program
    {
        static void fatalError(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadLine();
            Environment.Exit(41);
        }

        static void Main(string[] args)
        {
            EV3Wifi ev3 = new EV3Wifi();
            string conStatus = ev3.Connect();

            if (conStatus != "ok")
            {
                fatalError("Failed to connect to ev3");
            }
            else
            {
                Console.WriteLine("Ev3 connected");
            }

            Console.Out.Flush();


            while (true)
            {
                ev3.SendMessage("get_button", "STATUS");
                Thread.Sleep(200);
                string res = ev3.ReceiveMessage("EV3Wifi", "BUTTON");
                Console.WriteLine("Button: " + res);
                Console.Out.Flush();

                ConsoleKeyInfo cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }

            ev3.Disconnect();
        }
    }
}
