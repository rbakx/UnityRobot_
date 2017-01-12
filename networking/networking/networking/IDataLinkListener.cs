
namespace Networking
{
	public interface IDataLinkListener
	{
		bool Start (string address, short port);

		void Stop ();
	}
}