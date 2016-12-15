using Networking;

public class LocalDataLink : IDataLink
{

    public bool Connected()
    {
        return true;
    }

    public void Dispose()
    {

    }

    public bool SendData(byte[] data)
    {
        return true;
    }

    public void SetReceiver(IDataStreamReceiver receiver)
    {
        
    }
}
