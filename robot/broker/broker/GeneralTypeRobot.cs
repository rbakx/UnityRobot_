using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Networking;

namespace broker
{
    public abstract class GeneralTypeRobot
    {
        private string _robotName;
        private TypeRobot _robotType;

        protected Communicator _communicator;

        public GeneralTypeRobot(Communicator communicator, string name, TypeRobot type)
        {
            _robotName = name;
            _robotType = type;

            _communicator = communicator;
        }

        public void IdentificationRequest(int messageId)
        {
            Message message = MessageBuilder.CreateMessage(messageId, 
                MessageTarget_.Unity, MessageType_.IdentificationResponse);

            message.SetIdentificationResponse(_robotType.ToString());

            _communicator.SendCommand(message);
        }

        public virtual void OnNonStandardMessage(Message incomingMessage)
        {
            Message message = MessageBuilder.CreateMessage(
                MessageTarget_.Unity, MessageType_.LogError);

            message.SetLogError("Custom message is not implemented");

            _communicator.SendCommand(message);
        }

        public abstract void Indicate();
        public abstract void VelocitySet(Communication.Transform.Vector3_ linear, Communication.Transform.Vector3_ angular);
    }
}
