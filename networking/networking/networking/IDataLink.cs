using System;

namespace Networking
{
    public interface IDataLink : IDisposable
    {
        bool SendData(byte[] data);
        bool Connected();

		void SetReceiver(IDataStreamReceiver receiver);
    }

}
