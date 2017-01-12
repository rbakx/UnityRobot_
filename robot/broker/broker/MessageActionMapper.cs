using Communication;
using Networking;
using System;

namespace broker
{
    public class MessageActionMapper : IMessageReceiver
    {
        private GeneralTypeRobot _robot;

        public MessageActionMapper(GeneralTypeRobot robot)
        {
            _robot = robot;
        }

        public void IncomingMessage(Message newMessage, IDataLink dataLink)
        {
            Console.WriteLine("Received message: {0}", newMessage.messageType.ToString());

            switch (newMessage.messageType)
            {
                // When Unity detects a new robot a request for identification will be send.
                case MessageType_.IdentificationRequest:
                    _robot.IdentificationRequest(newMessage.id);
                    break;

                // Used to changed the linear and/or angular velocity of a robot.
                case MessageType_.VelocityChange:
                    _robot.VelocitySet(newMessage.robotVelocity);
                    break;

                case MessageType_.RotationRequest:
                    _robot.OnRotationRequest();
                    break;

                // Used to make the robot show some kind of signal.
                case MessageType_.Indicate:
                    _robot.Indicate();
                    break;

                // Message type for custom messages/events. This can be used to interact
                // with robot specific functions.
                case MessageType_.CustomMessage:
                    _robot.OnNonStandardMessage(newMessage);
                    break;
            }
        }
    }
}
