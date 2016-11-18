using Communication;

namespace Networking
{
    public interface IPresentationProtocolSerializer
    {
        byte[] MessageToBinaryData(Message messsage);
        Message BinaryDataToMessage(byte[] data);
    }
}
 