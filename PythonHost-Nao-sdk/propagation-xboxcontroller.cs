using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using PhysicalControllers;

partial class Program
{
    static Dictionary<int, List<XboxController>> xboxClassReferences = new Dictionary<int, List<XboxController>>();

    public static void _controller_register(XboxController obj)
    {
        if (obj != null)
        {
            List<XboxController> list = null;
            xboxClassReferences.TryGetValue(obj.GetId(), out list);

            if (list == null)
            {
                list = new List<XboxController>();
                xboxClassReferences.Add(obj.GetId(), list);
            }

            list.Add(obj);
        }
    }

    public static void _controller_unregister(XboxController obj)
    {
        if (obj != null)
        {
            List<XboxController> list = null;
            xboxClassReferences.TryGetValue(obj.GetId(), out list);

            if (list != null)
            {
                list.Remove(obj);
            }
        }
    }

    [DllExport("_controller_propagate_analog", CallingConvention = CallingConvention.Cdecl)]
    public static void _controller_propagate_analog(int id_controller, string id_button, int value)
    {
        List<XboxController> list = null;

        xboxClassReferences.TryGetValue(id_controller, out list);

        if (list != null)
        {
            foreach (XboxController con in list)
            {
                con.UpdateAnalogState(id_button, value);
            }
        }
    }

    [DllExport("_controller_propagate_digital", CallingConvention = CallingConvention.Cdecl)]
    public static void _controller_propagate_digital(int id_controller, string id_button, bool value)
    {
        List<XboxController> list = null;

        xboxClassReferences.TryGetValue(id_controller, out list);

        if(list != null)
        {
            Console.WriteLine(list);
            foreach(XboxController con in list)
            {
                con.UpdateDigitalState(id_button, value);
            }
        }
    }
}
