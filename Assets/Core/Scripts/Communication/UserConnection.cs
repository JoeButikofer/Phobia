using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public delegate void MessageReceive(UserConnection sender, string Data);

// The UserConnection class encapsulates the functionality of a TcpClient connection
// with streaming for a single user.
// From : http://wiki.unity3d.com/index.php/Simple_TCP/IP_Server_Code

public class UserConnection
{
    const int READ_BUFFER_SIZE = 1024;

    private TcpClient client;
    private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
    private string strName;

    private String currentMessage;

    // The Name property uniquely identifies the user connection.
    public string Name
    {
        get
        {
            return strName;
        }
        set
        {
            strName = value;
        }
    }

    public event MessageReceive MessageReceived;

    // Overload the new operator to set up a read thread.
    public UserConnection(TcpClient client)
    {
        this.client = client;

        currentMessage = "";

        // This starts the asynchronous read thread.  The data will be saved into
        // readBuffer.
        this.client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
    }

    // This subroutine uses a StreamWriter to send a message to the user.
    public void SendData(string Data)
    {
        //lock ensure that no other threads try to use the stream at the same time.
        lock (client.GetStream())
        {
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.Write(Data + (char)13 + (char)10);
            // Make sure all data is sent now.
            writer.Flush();
        }
    }

    // This is the callback function for TcpClient.GetStream.Begin. It begins an 
    // asynchronous read from a stream.
    private void StreamReceiver(IAsyncResult ar)
    {
        int BytesRead;
        string strMessage;

        try
        {
            
            // Ensure that no other threads try to use the stream at the same time.
            lock (client.GetStream())
            {
                // Finish asynchronous read into readBuffer and get number of bytes read.
                BytesRead = client.GetStream().EndRead(ar);
            }
            // Convert the byte array the message was saved into

            strMessage = Encoding.UTF8.GetString(readBuffer, 0, BytesRead);

            currentMessage += strMessage;

            // Wait message terminator ETX (0x3) before sending the entire message
            if (System.Convert.ToInt32(strMessage[strMessage.Length - 1]) == 0x3)
            {
                MessageReceived(this, currentMessage.Remove(currentMessage.Length - 1)); // Remove ETX
                currentMessage = "";
            }

            // Ensure that no other threads try to use the stream at the same time.
            lock (client.GetStream())
            {
                // Start a new asynchronous read into readBuffer.
                client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
            }
        }
        catch (Exception e)
        {
        }
    }
}