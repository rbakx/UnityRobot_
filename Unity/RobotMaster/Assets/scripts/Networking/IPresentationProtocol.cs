using Communication;

namespace Networking
{
    public interface IPresentationProtocol
    {
        void IncomingData(byte[] data);

        byte[] MessageToBinaryData(Message messsage);

        Message BinaryDataToMessage(byte[] data);
    }
}
 