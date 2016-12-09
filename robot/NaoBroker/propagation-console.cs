using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGiesecke.DllExport;
using System.IO;

partial class Program
{
    /*
        DebugWriter is a class that implements TextWriter.
        Implementing TextWriter allows for usage of this class as an output for console.
        This class converts debug messages to a string which is added to a list of messages.
    */
    public sealed class DebugWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        public override void WriteLine(string s)
        { Program.Log(s); }

        public override void WriteLine(object s)
        { Program.Log(s.ToString()); }

        public override void WriteLine(int s)
        { Program.Log(s.ToString()); }
    }


    public static void Exception(string errorMessage)
    {
        propagatedCommand cmd;
        cmd.command = "except|" + errorMessage;
        cmd.callback = null;
        cmd.signal = null;

        commands.Add(++commandIdentity, cmd);
    }

    public static void Log(string errorMessage)
    {
        propagatedCommand cmd;
        cmd.command = "log|" + errorMessage;
        cmd.callback = null;
        cmd.signal = null;

        commands.Add(++commandIdentity, cmd);
    }
}