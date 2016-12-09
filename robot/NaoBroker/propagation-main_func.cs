using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using System.Threading;

partial class Program
{
    /*
        Forward declare a main so this method can be called from within Python.
        A main is declared to take away the feeling of working with a library rather than a C# program,
        and allowing for programming in C#.
    */
    static partial void main(string[] arguments);

    [DllExport("_main", CallingConvention = CallingConvention.Cdecl)]
    public static void _main(string arguments)
    {
        try
        {
            DebugWriter ws = new DebugWriter();

            Console.SetOut(ws);

            string[] args = arguments.Split('|');

            Console.WriteLine("Library loaded");

            main(args);

            Console.WriteLine("Library main ended");
        }
        catch (Exception e)
        { Program.Exception(e.ToString()); }


        Thread.Sleep(7);

        propagatedCommand cmd;
        cmd.command = "exit";
        cmd.callback = null;
        cmd.signal = null;

        commands.Add(++commandIdentity, cmd);
    }
}
