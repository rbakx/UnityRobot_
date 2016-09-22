namespace Networking
{
    public interface IPresentationProtocol
    {
        void IncomingData(string data);

        string MessageToBinaryData(Message messsage);

        Message BinaryDataToMessage(string data);
    }
}
 