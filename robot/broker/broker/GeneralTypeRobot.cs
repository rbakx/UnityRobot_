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
            Message message = new Message
            {
                id = messageId,
                messageTarget = MessageTarget_.Unity,
                messageType = MessageType_.IdentificationResponse
            };

            message.identificationResponse.robotType = _robotType.ToString();

            _communicator.SendCommand(message);
        }

        public virtual void OnNonStandardMessage(Message incomingMessage)
        {
            Message message = new Message
            {
                id = incomingMessage.id,
                messageTarget = MessageTarget_.Unity,
                messageType = MessageType_.LogError
            };
            
            message.error.message = "Custom message is not implemented";

            _communicator.SendCommand(message);
        }

        public abstract void Indicate();
        public abstract void VelocitySet(Communication.Transform.Vector3_ linear, Communication.Transform.Vector3_ angular);
    }
}
