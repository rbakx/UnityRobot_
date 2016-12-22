using Communication;
using Communication.Messages;
using Networking;
using System;

namespace broker
{
    public abstract class GeneralTypeRobot : IDisposable
    {
        private string _robotName;
        private TypeRobot _robotType;

        protected Communicator _communicator;

        public GeneralTypeRobot(Communicator communicator, string name, TypeRobot type)
        {
			if (communicator == null)
			{
				throw new ArgumentNullException("communicator");
			}

			if (!communicator.GetDataLink().Connected())
			{
				throw new InvalidOperationException("Datalink in communicator not connected");
			}

			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

            _robotName = name;
            _robotType = type;

            _communicator = communicator;
        }

        ~GeneralTypeRobot()
        {
            Dispose();
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

        public void OnRotationRequest()
        {
            float rotation = GetRotation();

            Message response = MessageBuilder.CreateMessage(
                MessageTarget_.Unity, MessageType_.RotationResponse);

            response.SetRotationResponse(rotation);

            _communicator.SendCommand(response);
        }

        public void Disconnect()
        {
            Message message = MessageBuilder.CreateMessage(
                MessageTarget_.Unity, MessageType_.Disconnect);

            _communicator.SendCommand(message);
        }

        public abstract void Indicate();
        public abstract void VelocitySet(SetVelocity_ velocity);
        public abstract float GetRotation();

        public void Dispose()
        {
            _communicator.Dispose();
        }
    }
}
