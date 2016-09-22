namespace Networking
{
    public interface IMessageReceiver
    {
        void IncomingMessage(Message newMessage);
    }
}
