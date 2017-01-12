using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace nao_broker
{
    public class NaoConnection
    {
        private int LocalIdentity;

        public NaoConnection(string address, short port, int timeout = 10000)
        {
            LocalIdentity = -1;

            ManualResetEvent oSignalEvent = new ManualResetEvent(false);

            Delegate returnval = new _Nao_incoming_result_processing(Nao_incoming_result_processing);

            Program.NaoRequestConnection(address, port, oSignalEvent, returnval);

            //wait until response is returned!
            if (oSignalEvent.WaitOne(timeout))
            {
                if (LocalIdentity < 0)
                {
                    throw new Exception("[NaoBroker] Could not connect with Nao robot at " + address + ":" + port);
                }
            }
            else
            {
                throw new Exception("[NaoBroker] library signal not received!");
            }
        }

        public delegate void _Nao_incoming_result_processing(string ID);

        public int GetId()
        {
            return LocalIdentity;
        }

        private void Nao_incoming_result_processing(string ID)
        {
            if (!int.TryParse(ID, out LocalIdentity))
            {
                LocalIdentity = -1;
            }
        }

        public bool IsConnected()
        {
            return LocalIdentity >= 0;
        }

        public void Motion_MoveForward(float x = 0.0F, float y = 0.0F, float theta = 0.0F, float frequency = 1.0F)
        {
            if (LocalIdentity > 0)
            {
                Program.Nao_Motion_MoveForward(LocalIdentity, x, y, theta, frequency);
            }
        }

        public void Motion_SetHandState(string handName = "RHand", bool state = false)
        {
            if (LocalIdentity > 0)
            {
                Program.Nao_Motion_SetHandState(LocalIdentity, handName, state);
            }
        }

        public void Motion_MoveStop()
        {
            if (LocalIdentity > 0)
            {
                Program.Nao_Motion_MoveStop(LocalIdentity);
            }
        }

        public void Posture_GoTo(string postureName = "StandInit", bool waitForActionComplete = false)
        {
            if (LocalIdentity > 0)
            {
                if (waitForActionComplete)
                {
                    ManualResetEvent ev = new ManualResetEvent(false);
                    Program.Nao_Posture_GoTo(LocalIdentity, postureName, ev);
                    ev.WaitOne();
                }
                else
                {
                    Program.Nao_Posture_GoTo(LocalIdentity, postureName);
                }
            }
        }

        public void Motion_AngleInterpolation(string name, float targetAngle = 0.0F, float targetTime = 0.5F, bool isAbsolute = false)
        {
            if (LocalIdentity > 0)
            {
                Program.Nao_Motion_AngleInterpolation(LocalIdentity, name, targetAngle, targetTime, isAbsolute);
            }
        }

        public void Motion_StiffnessInterpolation(string name, float stiffness = 0.0F, float targetTime = 0.5F)
        {
            if (LocalIdentity > 0)
            {
                Program.Nao_Motion_StiffnessInterpolation(LocalIdentity, name, stiffness, targetTime);
            }
        }

        public void Motion_WakeUp(bool waitForActionComplete = false)
        {
            if (LocalIdentity > 0)
            {
                if (waitForActionComplete)
                {
                    ManualResetEvent ev = new ManualResetEvent(false);
                    Program.Nao_Motion_WakeUp(LocalIdentity, ev);

                    Console.WriteLine("waiting");

                    ev.WaitOne();
                }
                else
                {
                    Program.Nao_Motion_WakeUp(LocalIdentity);
                }


            }
        }

        public void Motion_Rest(bool waitForActionComplete = false)
        {
            if (LocalIdentity > 0)
            {
                if (waitForActionComplete)
                {
                    ManualResetEvent ev = new ManualResetEvent(false);
                    Program.Nao_Motion_Rest(LocalIdentity, ev);

                    Console.WriteLine("waiting");

                    ev.WaitOne();
                }
                else
                {
                    Program.Nao_Motion_Rest(LocalIdentity);
                }
            }
        }

        public void TSS_Say(string text = "")
        {
            if (LocalIdentity > 0)
            {
                Program.Nao_TSS_Say(LocalIdentity, text);
            }
        }
    }
}
