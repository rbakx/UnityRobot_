using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

partial class Program
{
   public static int NaoRequestConnection(string ip, short port, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;
        cmd.command = "nao_connect|" + ip + "|" + port;
        cmd.callback = del;
        cmd.signal = ev;

        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Motion_MoveStop(int localIdentityId, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;

        cmd.command = "nao_motion_movestop|" + localIdentityId;
        cmd.callback = del;
        cmd.signal = ev;

        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Motion_MoveForward(int localIdentityId, float x, float y, float theta, float frequency, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;

        cmd.command = "nao_motion_moveforward|" + localIdentityId + "|" + x.ToString() + "|" + y.ToString() + "|" + theta.ToString() + "|" + frequency.ToString();
        cmd.callback = del;
        cmd.signal = ev;

        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_TSS_Say(int localIdentityId, string text, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;
        cmd.command = "nao_tss_say|" + localIdentityId + "|" + text;
        cmd.callback = del;
        cmd.signal = ev;

        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Posture_GoTo(int localIdentityId, string poseName, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;
        cmd.command = "nao_posture_goto|" + localIdentityId + "|" + poseName;
        cmd.callback = del;
        cmd.signal = ev;

        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Motion_SetHandState(int localIdentityId, string handName, bool state, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;

        cmd.command = "nao_motion_sethandstate|" + localIdentityId + "|" + handName + "|" + state;
        cmd.callback = del;

        cmd.signal = ev;
        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Motion_StiffnessInterpolation(int localIdentityId, string name, float stiffness = 0.0F, float targetTime = 1.0F, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;

        cmd.command = "nao_motion_stiffness_interpolation|" + localIdentityId + "|" + name + "|" + stiffness + "|" + targetTime;
        cmd.callback = del;

        cmd.signal = ev;
        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Motion_AngleInterpolation(int localIdentityId, string name, float targetAngle = 0.0F, float targetTime = 1.0F, bool isAbsolute = false, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;

        cmd.command = "nao_motion_angle_interpolation|" + localIdentityId + "|" + name + "|" + targetAngle + "|" + targetTime + "|" + isAbsolute;
        cmd.callback = del;

        cmd.signal = ev;
        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Motion_WakeUp(int localIdentityId, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;
        cmd.command = "nao_motion_setwakestate|" + localIdentityId+"|True";
        cmd.callback = del;

        cmd.signal = ev;

        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }

    public static int Nao_Motion_Rest(int localIdentityId, ManualResetEvent ev = null, Delegate del = null)
    {
        propagatedCommand cmd;
        cmd.command = "nao_motion_setwakestate|" + localIdentityId+"|False";
        cmd.callback = del;

        cmd.signal = ev;
        commands.Add(++commandIdentity, cmd);

        return commandIdentity;
    }
}
