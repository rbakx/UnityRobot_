using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Networking
{
	public class TCPDataLinkListener<T> : IDataLinkListener, IDisposable
		where T : IPresentationProtocol, new()

	{
        public const int NO_CONNECTION_PENDING_TIMEOUT = 2; // (ms)

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
            if (!_listening)
            {
                try
                {
                    _listener = new TcpListener(IPAddress.Parse(address), (int)port);
                    _listener.Start();

                    _listenerThread = new Thread(Listen);
                    _listenerThread.Start(this);

                    _listening = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Stop();
                }          
            }

			return _listening;
		}

		public void Stop()
		{
			_listening = false;

            if (_listener != null)
            { _listener.Stop(); }

            if (_listenerThread != null)
            {
                _listenerThread.Join();

                _listenerThread = null;
            }
        }

		private static void Listen(object obj)
		{
			TCPDataLinkListener<T> listener = obj as TCPDataLinkListener<T>;

			while(listener._listening)
			{
				while(listener._listener.Pending())
				{
					TcpClient newClient = listener._listener.AcceptTcpClient();
					TCPDataLink newDataLink = new TCPDataLink(newClient);
					T pp = new T();
					newDataLink.SetReceiver(pp);

					listener._subscriber.IncomingNewDataLink(newDataLink, pp);
				}

                if(!listener._listening)
                    Thread.Sleep(NO_CONNECTION_PENDING_TIMEOUT);
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
