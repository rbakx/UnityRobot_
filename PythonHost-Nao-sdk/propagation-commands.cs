using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RGiesecke.DllExport;
using System.IO;
using System.Threading;

partial class Program
{
    public struct propagatedCommand
    {
        public string command;
        public Delegate callback;
        public ManualResetEvent signal;
    }

    /*
    
    */
    private static int commandIdentity = 0;

    /*
    
    */
    static Dictionary<int, propagatedCommand> commands = new Dictionary<int, propagatedCommand>();

    /*
        
    */
    static Dictionary<int, propagatedCommand> propagatedCommands = new Dictionary<int, propagatedCommand>();

    [DllExport("RequestCompleted", CallingConvention = CallingConvention.Cdecl)]
    public static void RequestCompleted(string data)
    {
        int index = -1;

        int commandIdentity = -1;

        string[] parts = null;

        parts = data.Split('|');

        if (parts.Length > 0 && int.TryParse(parts[0], out commandIdentity))
        {

            for (int i = 0; i < propagatedCommands.Count; ++i)
            {
                if (propagatedCommands.Keys.ElementAt(0) == commandIdentity) { index = i; break; };
            }

            if (index == 0)
            {
                propagatedCommand command = propagatedCommands.ElementAt(index).Value;

                if (command.callback != null)
                {
                    parts = parts.Skip(1).ToArray();

                    command.callback.DynamicInvoke(parts);
                }

                if (command.signal != null)
                {
                    command.signal.Set();
                }
            }
        }
    }

    [DllExport("CallCycle", CallingConvention = CallingConvention.Cdecl)]
    public static string CallCycle()
    {
        string commandIdentity = "";

        if(commands.Count > 0)
        {
            KeyValuePair<int, propagatedCommand> p = commands.ElementAt(0);

            commandIdentity = p.Key + "|" + p.Value.command;

            if (p.Value.signal != null || p.Value.callback != null)
            { propagatedCommands.Add(p.Key, p.Value); }

            commands.Remove(p.Key);
        }

        return commandIdentity;
    }
}