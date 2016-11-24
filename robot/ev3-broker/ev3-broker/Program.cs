using EV3WifiLib;
using System;
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

        static bool CurrentKey(ConsoleKey key)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo cKey = Console.ReadKey(true);

                return key == cKey.Key;
            }
            else
            {
                return false;
            }
        }

        static void Main(string[] args)
        {
            using (EV3 ev3 = new EV3("EV3Wifi"))
            {
                if (!ev3.Connect(5000))
                {
                    fatalError("No EV3 devices detected.");
                }
                else
                {
                    Console.WriteLine("EV3 detected: " + ev3.SerialNumber);
                }

                ev3.SendMessage("DISPLAY", "Reading button");

                while (true)
                {
                    ev3.SendMessage("STATUS", "get_button");
                    string result = ev3.ReceiveMessage("BUTTON");

                    Console.WriteLine(result);

                    //Console.Write("speed: ");

                    //string speedStr = Console.ReadLine();

                    //float speed = 0;
                    //if (!float.TryParse(speedStr, out speed))
                    //{
                    //    Console.WriteLine("Invalid input");
                    //}
                    //else if (!ev3.SendMessage("SPEED", speed))
                    //{
                    //    Console.WriteLine("Message send failed");
                    //}


                    
                    if (CurrentKey(ConsoleKey.Escape))
                    {
                        break;
                    }
                }
            }

            Console.ReadLine();


            //EV3Wifi ev3 = new EV3Wifi();
            //string conStatus = ev3.Connect();

            //if (conStatus != "ok")
            //{
            //    fatalError("Failed to connect to ev3");
            //}
            //else
            //{
            //    Console.WriteLine("Ev3 connected");
            //}

            //Console.Out.Flush();

            //ev3.SendMessage("TEST", "DISPLAY");


            //while (true)
            //{
            //    ev3.SendMessage("get_button", "STATUS");
            //    Thread.Sleep(200);
            //    string res = ev3.ReceiveMessage("EV3Wifi", "BUTTON");
            //    Console.WriteLine("Button: " + res);
            //    Console.Out.Flush();

            //    ConsoleKeyInfo cki = Console.ReadKey();
            //    if (cki.Key == ConsoleKey.Escape)
            //    {
            //        break;
            //    }
            //}

            //ev3.Disconnect();
        }
    }
}
