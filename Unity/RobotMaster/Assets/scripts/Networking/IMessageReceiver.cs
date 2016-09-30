using Communication;

namespace Networking
{
    public interface IMessageReceiver
    {
        void IncomingMessage(Message newMessage, IDataLink dataLink);
    }
}
