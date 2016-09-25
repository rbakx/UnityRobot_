using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Networking
{
	public class TCPDataLinkListener<T> : IDataLinkListener, IDisposable
		where T : IPresentationProtocol, new()

	{
        public const int NO_CONNECTION_PENDING_TIMEOUT = 10; // (ms)

		private TcpListener _listener;
		private Thread _listenerThread;
		private bool _listening;

		private bool _disposed;

		private IIncomingDataLinkSubscriber _subscriber;


		public TCPDataLinkListener(IIncomingDataLinkSubscriber subscriber)
		{
			if(subscriber == null)
			{
				throw new ArgumentNullException("subscriber");
			}

			_subscriber = subscriber;
			_listening = false;
			_disposed = false;
		}

		public bool Start(string address, short port)
		{
			try
			{
				_listener = new TcpListener(IPAddress.Parse(address), (int)port);
                _listener.Start();

				_listenerThread = new Thread(Listen);
				_listening = true;
				_listenerThread.Start(this);
			}
			catch(ArgumentOutOfRangeException ex)
			{
				return false;
			}

			return true;
		}

		public void Stop()
		{
			if(_listening)
			{
				_listening = false;
				_listener.Stop();
				_listenerThread.Join();
			}
		}

		private static void Listen(object obj)
		{
			TCPDataLinkListener<T> listener = obj as TCPDataLinkListener<T>;

			while(listener._listening)
			{
				if(listener._listener.Pending())
				{
					TcpClient newClient = listener._listener.AcceptTcpClient();
					TCPDataLink newDataLink = new TCPDataLink(newClient);
					T pp = new T();

					listener._subscriber.IncomingNewDataLink(newDataLink, pp);
				}
                else
                {
                    Thread.Sleep(NO_CONNECTION_PENDING_TIMEOUT);
                }
			}
		}

		public void Dispose ()
		{
			if (!_disposed)
			{
				_disposed = true;

				Stop();
			}
		}
	}
}