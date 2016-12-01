using broker;
using Communication.Transform;
using Networking;
using System;

namespace ev3_broker
{
    class EV3Robot : GeneralTypeRobot, IDisposable
    {
        private Ev3Connection _ev3Connection;

        private bool _disposed = false;

        public EV3Robot(Communicator communicator, string name, Ev3Connection ev3Connection) : base(communicator, name, TypeRobot.Mindstorm)
        {
            if (ev3Connection == null)
            {
                throw new ArgumentNullException("ev3Connection");
            }
            _ev3Connection = ev3Connection;
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
