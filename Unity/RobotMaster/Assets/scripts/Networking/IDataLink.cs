﻿
namespace Networking
{
    public interface IDataLink
    {
        bool SendData(byte[] data);
        bool Connected();

		void SetReceiver(IDataStreamReceiver receiver);
    }

}
