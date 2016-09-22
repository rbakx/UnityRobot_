using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class SocketTest : MonoBehaviour
{

    private TcpListener listener;

    // Use this for initialization
    void Start()
    {

        IPAddress ipAddress = null;

        #region Get ip4 from hostname
        string name = Dns.GetHostName();

        try
        {
            foreach (IPAddress addr in Dns.GetHostEntry(name).AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    Debug.Log(string.Format("{0}/{1}", name, addr));
                    ipAddress = addr;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        #endregion

        listener = new TcpListener(IPAddress.Parse("145.93.69.247"), 1234);
        listener.Start();

        //using (TcpClient socket = listener.AcceptTcpClient())
        //{
        //    Debug.Log(">> Connection accepted.");
        //    var stream = socket.GetStream();


        //    byte[] receivedData = new byte[1025];
        //    stream.Read(receivedData, 0, 1024);
        //    string stringData = Encoding.ASCII.GetString(receivedData);
        //    Debug.Log(stringData);

        //}

        StartAccept();
    }

    private void StartAccept()
    {
        listener.BeginAcceptTcpClient(HandleAsyncConnection, listener);
    }

    private void HandleAsyncConnection(IAsyncResult res)
    {
        StartAccept();

        TcpClient client = listener.EndAcceptTcpClient(res);
        var stream = client.GetStream();

        Debug.Log("Connection accepted");

        while (true)
        {
            // Recieve a message
            byte[] receivedData = new byte[1024];
            stream.Read(receivedData, 0, 1024);
            string stringData = Encoding.ASCII.GetString(receivedData);
            Debug.Log(string.Format("Recieved message: {0}", stringData));

            // Echo the message
            byte[] sendData = Encoding.ASCII.GetBytes(stringData);
            stream.Write(sendData, 0, sendData.Length);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
