namespace Networking
{
    public interface IMessageSender
    {
        bool SendCommand(Message message);
    }
}