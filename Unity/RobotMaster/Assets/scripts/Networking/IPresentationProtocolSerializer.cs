using Communication;

namespace Networking
{
    public interface IPresentationProtocolSerializer
    {
        void IncomingData(byte[] data);

        byte[] MessageToBinaryData(Message messsage);

        Message BinaryDataToMessage(byte[] data);
    }
}
 