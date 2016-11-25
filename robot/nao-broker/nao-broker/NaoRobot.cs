using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using Communication;
using broker;

namespace nao_broker
{
    public class NaoRobot : GeneralTypeRobot
    {
        public NaoRobot(Communicator communicator, string name) : base(communicator, name, TypeRobot.Nao)
        {

        }

        public override void Indicate()
        {

        }

        public override void VelocitySet(Communication.Transform.Vector3_ linear, Communication.Transform.Vector3_ angular)
        {
            throw new NotImplementedException();
        }

        public override void OnNonStandardMessage(Message incomingMessage)
        {
            throw new NotImplementedException();
        }
    }
}
