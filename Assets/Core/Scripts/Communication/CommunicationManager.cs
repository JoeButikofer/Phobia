using System;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine;

public delegate void CommandReceived(string data);

public class CommunicationManager {


    private static CommunicationManager instance;

    public static CommunicationManager Instance
    {
        get
        {
            if (instance == null)
                instance = new CommunicationManager();

            return instance;
        }
    }

    public event CommandReceived commandReceived;

    const int PORT_NUM = 10000;
    TcpListener listener = null;

    private bool isListening;
    private Thread listenerThread;

    private CommunicationManager()
    {
        
    }

    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, PORT_NUM);
        listenerThread = new Thread(new ThreadStart(DoListen));
        listenerThread.Start();
        isListening = true;
    }

    public void Stop()
    {
        isListening = false;
    }

    private void DoListen()
    {
        try
        {
            // Listen for new connections.
            listener = new TcpListener(IPAddress.Any, PORT_NUM);
            listener.Start();
            
            do
            {
                if (listener.Pending())
                {
                    // Create a new user connection using TcpClient returned by
                    // TcpListener.AcceptTcpClient()
                    UserConnection client = new UserConnection(listener.AcceptTcpClient());

                    client.MessageReceived += new MessageReceive(OnMessageReceived);
                }

                Thread.Sleep(100);
            } while (isListening);
        }
        catch (Exception ex)
        {
            Debug.Log("Listener Exception : " + ex);
        }
        finally
        {
            if(listener != null)
                listener.Stop();
        }
    }

    private void OnMessageReceived(UserConnection sender, string data)
    {
        commandReceived(data);
    }
}
