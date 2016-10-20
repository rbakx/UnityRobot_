namespace Networking
{
	public interface IIncomingDataLinkSubscriber
	{

		void IncomingNewDataLink (IDataLink dataLink, 
		                          IPresentationProtocol usedProtocol);
	}
}