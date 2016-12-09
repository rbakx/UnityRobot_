using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using broker;
using Communication.Transform;

namespace nao_broker
{
    public class NaoRobot : GeneralTypeRobot, IDisposable 
    {
        private NaoConnection _naoConnection;
        private bool _disposed = false;

        public NaoRobot(Communicator communcator, string name, NaoConnection naoConnection) : base(communcator, name, TypeRobot.Nao)
        {
            _naoConnection = naoConnection;
        }

        public override void Indicate()
        {
            _naoConnection.TSS_Say("Indicate");   
        }

        public override void VelocitySet(Vector3_ linear, Vector3_ angular)
        {
            //throw new NotImplementedException();
            Vector3_ velocities = new Vector3_();

            _naoConnection.Motion_MoveForward(velocities.x, velocities.y, velocities.z);
        }

        public void Dispose()
        {
            if(!_disposed)
            {
                _disposed = true;
                _communicator.GetDataLink().Dispose();
                //_naoConnection.Dispose();
            }
        }
    }
}
