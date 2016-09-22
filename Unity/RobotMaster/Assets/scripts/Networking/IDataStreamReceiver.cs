

namespace Networking
{
    public interface IDataStreamReceiver
    {
        void IncomingData(byte[] data);
    }

}
