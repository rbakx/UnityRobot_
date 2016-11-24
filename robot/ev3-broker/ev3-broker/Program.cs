using System;
using System.Threading;

namespace ev3_broker
{
    class Program
    {
        volatile static bool running = true;

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
            Thread ev3Thread = new Thread(EV3Work);
            ev3Thread.Start();

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo cKey = Console.ReadKey(true);

                    switch (cKey.Key)
                    {
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                    }
                }
            }

            ev3Thread.Join();
        }

        static void EV3Work()
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

                while (running)
                {
                    if (running ==false)
                    {
                        Console.WriteLine("wha");
                    }
                    ev3.SendMessage("STATUS", "get_button");
                    string result = ev3.ReceiveString("BUTTON");

                    Console.WriteLine(result);
                }
            }
        }
    }
}
