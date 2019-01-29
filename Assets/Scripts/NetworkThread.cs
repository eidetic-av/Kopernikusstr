using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetworkThread
{
    public static UdpClient Client { get; private set; }
    static IPEndPoint _ipEndPoint;
    public static IPEndPoint IPEndPoint { get { return _ipEndPoint; } }

    public static Queue<byte[]> ReceivedData = new Queue<byte[]>();

    public static bool Connect(int port)
    {
        var started = false;
        _ipEndPoint = new IPEndPoint(IPAddress.Any, port);
        Client = new UdpClient(port);
        try
        {
            Client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            started = true;
        }
        catch (Exception e) { }
        return started;
    }

    public static void Disconnect()
    {
        if (Client != null)
        {
            Client.Close();
            Client.Dispose();
            Client = null;
        }
    }

    static void ReceiveCallback(IAsyncResult res)
    {
        // collect the received bytes and add to the message queue
        byte[] received = Client.EndReceive(res, ref _ipEndPoint);
        ReceivedData.Enqueue(received);
        // start receiving again from scratch
        Client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }
}
