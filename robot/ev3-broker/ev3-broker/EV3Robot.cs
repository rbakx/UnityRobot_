using broker;
using Communication.Transform;
using Networking;
using System;

namespace ev3_broker
{
    class EV3Robot : GeneralTypeRobot, IDisposable
    {
        private EV3 _ev3Connection;

        private bool _disposed = false;

        public EV3Robot(Communicator communicator, string name) : base(communicator, name, TypeRobot.Mindstorm)
        {
            _ev3Connection = new EV3("EV3Wifi");
            if (!_ev3Connection.Connect(5000))
            {
                throw new Exception("Failed to connect with ev3");
            }
        }

        public override void Indicate()
        {
            _ev3Connection.SendMessage("INDICATE", " ");
        }

        public override void VelocitySet(Vector3_ linear, Vector3_ angular)
        {
            if (linear != null)
            {
                _ev3Connection.SendMessage("LVEL", linear.x);
            }

            if (angular != null)
            {
                _ev3Connection.SendMessage("AVEL", angular.z);
            }
        }

        public void Dispose()
        {
            if(!_disposed)
            {
                _disposed = true;
                _ev3Connection.Dispose();
            }
        }
    }
}
