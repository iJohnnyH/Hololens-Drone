using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif
public class Client : MonoBehaviour
{
#if UNITY_EDITOR
    void Start(){}
     // Update is called once per frame
    void Update(){}
#endif
#if WINDOWS_UWP
    public StreamSocket socket;
    // Use this for initialization
    async void Start()
    {
        Debug.Log("---STARTING CLIENT---");
        await connect("152.23.42.40", "5717");
        Debug.Log("---SENDING MESSEGES---");
        while (true)
        {
            //sendCameraRotation();
            Debug.Log(transform.eulerAngles.x + " " + transform.eulerAngles.y + " " + transform.eulerAngles.z);
            Task.Delay(5000);
        }
        await send("QUIT");
        socket.Dispose();
        Debug.Log("---ENDING CLIENT---");
    }

    async public void sendCameraRotation()
    {
        /* await send("s");
         Task.Delay(5);
         await send(transform.rotation.eulerAngles.x.ToString());
         Task.Delay(5);
         await send(transform.rotation.eulerAngles.y.ToString());
         Task.Delay(5);
         await send(transform.rotation.eulerAngles.z.ToString());
         Task.Delay(5);*/
    }

    public async Task connect(string host, string port)
    {
        Debug.Log("---ATTEMPTING TO CONNECT TO SERVER");
        HostName hostName;
        socket = new StreamSocket();
        hostName = new HostName(host);

        // Set NoDelay to false so that the Nagle algorithm is not disabled
        socket.Control.NoDelay = false;
        try
        {
            // Connect to the server
            await socket.ConnectAsync(hostName, port);
            Debug.Log("---SUCCESFULLY CONNECTED TO SERVER---");
        }
        catch (Exception exception)
        {
            switch (SocketError.GetStatus(exception.HResult))
            {
                case SocketErrorStatus.HostNotFound:
                    Debug.Log("---HOST NOT FOUND---");
                    throw;
                default:
                    System.Diagnostics.Debug.WriteLine(exception.ToString());
                    Debug.Log("--------------");
                    throw;
            }
        }
    }

    public async Task send(string message)
    {
        DataWriter writer = new DataWriter(socket.OutputStream);
        // Set the Unicode character encoding for the output stream
        writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
        // Specify the byte order of a stream.
        writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

        // Gets the size of UTF-8 string.
        writer.MeasureString(message);
        // Write a string value to the output stream.
        writer.WriteString(message);

        // Send the contents of the writer to the backing stream.
        try
        {
            await writer.StoreAsync();
            Debug.Log("---" + message + " SENT SUCCESFULLY---");
        }
        catch (Exception exception)
        {
            switch (SocketError.GetStatus(exception.HResult))
            {
                case SocketErrorStatus.HostNotFound:
                    Debug.Log("---HOST NOT FOUND---");
                    throw;
                default:
                    System.Diagnostics.Debug.WriteLine(exception.ToString());
                    Debug.Log("---ERROR SENDING " + message + "---");
                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    throw;
            }
        }

        await writer.FlushAsync();
        // In order to prolong the lifetime of the stream, detach it from the DataWriter
        writer.DetachStream();
        writer.Dispose();
    }
#endif
}
