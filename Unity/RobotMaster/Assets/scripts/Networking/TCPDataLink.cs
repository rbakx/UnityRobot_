using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Networking
{

    public class TCPDataLink : IDataLink
	{
		private TcpClient _client;
		private NetworkStream _stream;
		private Thread _readerThread;
		private bool _reading;


		private IDataStreamReceiver _receiver;

		private bool disposed = false;

		public TCPDataLink (TcpClient client)
		{
			if (client == null)
			{
				throw new ArgumentNullException ("client");
			}

			if (!client.Connected)
			{
				throw new InvalidOperationException ("TCPDataLink: Client is not connect to a socket!");
			}

			_client = client;
			_stream = client.GetStream ();
			_reading = false;
		}

		public void SetReceiver (IDataStreamReceiver receiver)
		{
			if (receiver == null)
			{
				throw new ArgumentNullException ("receiver");
			}

			_receiver = receiver;

			if (!_reading)
			{
				_readerThread = new Thread (Read);
				_readerThread.Start (this);
				_reading = true;
			}
		}

		public bool Connected ()
		{
			return _client.Connected;
		}

		public bool SendData (string data)
		{
			return SendData (Encoding.ASCII.GetBytes (data));
		}

		public bool SendData (byte[] data)
		{
			bool result = false;

			if (_client.Connected && _stream.CanWrite)
			{
				_stream.Write (data, 0, data.Length);

				result = true;
			}

			return result;
		}

		static private void Read (object datalink_obj)
		{
			TCPDataLink datalink = (TCPDataLink)datalink_obj;

			List<byte> buffer = new List<byte> ();
			byte[] bytebuffer = new byte[1];

			while (datalink.Connected ())
			{
				if (datalink._stream.DataAvailable)
				{
					if (datalink._stream.CanRead)
					{
						buffer.Clear ();

						while (datalink._stream.DataAvailable)
						{
							// TODO: maybe check byte receive count
							datalink._stream.Read (bytebuffer, 0, bytebuffer.Length);
							buffer.Add (bytebuffer [0]);
						}

						datalink._receiver.IncomingData (buffer.ToArray (), datalink);
					}
				}
			}
		}

		public void Dispose ()
		{
			if (!disposed)
			{
				disposed = true;

				_reading = false;

				_client.Close ();

                if (_readerThread != null)
                {
                    _readerThread.Join();
                }
			}
		}
	}
}
