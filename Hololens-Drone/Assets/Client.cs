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
    public Camera mainCamera;
    public String angles;
#if UNITY_EDITOR
    void Start(){}
     // Update is called once per frame
    void Update(){}
#endif
    public void updateCameraAngles()
    {
        float roll = mainCamera.transform.rotation.eulerAngles.z, pitch = mainCamera.transform.rotation.eulerAngles.x, yaw = mainCamera.transform.rotation.eulerAngles.y;
        roll = convertRoll(roll);
        pitch = convertPitch(pitch);
        yaw = convertYaw(yaw);
        angles = roll.ToString("F") + " " + pitch.ToString("F") + " " + yaw.ToString("F");
        //Debug.Log(mainCamera.transform.rotation.eulerAngles.z.ToString() + " " + mainCamera.transform.rotation.eulerAngles.x.ToString() + " " + mainCamera.transform.rotation.eulerAngles.y.ToString());
    }

    private float convertRoll(float roll)
    {
        if (roll > 35 && roll <= 180)
        {
            roll = 35;
        }
        else if (roll > 180 && roll < 325)
        {
            roll = -35;
        }
        else if (roll >= 325)
        {
            roll = roll - 360;
        }

        return roll * 10;
    }

    private float convertPitch(float pitch)
    {
        //Range checks
        if (pitch > 90 && pitch < 180)
        {
            pitch = 90;
        }
        else if (pitch > 180 && pitch < 330)
        {
            pitch = 330;
        }
        //Convert to drone angles [-900, 300], 30 degrees above horizontal, 90 degrees below horizontal
        if (pitch <= 90)
        {
            pitch *= -1;
        }
        else if (pitch >= 330)
        {
            pitch = 360 - pitch;
        }
        //Range checks
        return pitch * 10;
    }

    private float convertYaw(float yaw)
    {
        if (yaw >= 0 && yaw <= 180)
        {

        }
        else if (yaw > 180)
        {
            yaw = yaw - 360;
        }
        return yaw * 10;
    }

#if WINDOWS_UWP
    public StreamSocket socket;
    String avail = "TRUE";

    // Use this for initialization
    /*
     * Hololens Rotations
     * X:
     */

    async public void Start()
    {
        Debug.Log("---STARTING CLIENT---");
        angles = "0 0 0";
        InvokeRepeating("updateCameraAngles", 0, 1);
        //await connect("192.168.43.159", "5717");
        await connect("152.23.48.106", "5771");
        Debug.Log("---SENDING MESSEGES---");
        while (true)
        {
            await send("s");
            await waitAvail();
            await send(angles);
            await waitAvail();
        }
        socket.Dispose();
        Debug.Log("---ENDING CLIENT---");
        await send("QUIT");
    }

    async private Task sendCameraAngles()
    {
        await send("s");
        await waitAvail();
        float roll = mainCamera.transform.rotation.eulerAngles.z, pitch = mainCamera.transform.rotation.eulerAngles.x, yaw = mainCamera.transform.rotation.eulerAngles.y;
        await send(roll.ToString("F") + " " + pitch.ToString("F") + " " + yaw.ToString("F"));
        await waitAvail();
    }

    async private Task sendCameraAngles(double x, double y, double z)
    {
        await send("s");
        await Task.Delay(10);
        await send(x.ToString() + " " + y.ToString() + " " + z.ToString());
        avail = "";
    }

    private async Task sendRandomCameraAngles()
    {
        System.Random seed = new System.Random();
        float roll = (float)seed.Next(-350, 351);
        float pitch = (float)seed.Next(-900, 301);
        float yaw = (float)seed.Next(-3200, 3201);
        await send("s");
        await Task.Delay(10);
        await send(roll.ToString() + " " + pitch.ToString() + " " + yaw.ToString());
        avail = "";
    }

    async private Task connect(string host, string port)
    {
        Debug.Log("---ATTEMPTING TO CONNECT TO SERVER---");
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

    async private Task send(string message)
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
        avail = "";
    }

    async private Task recieve()
    {
        DataReader reader = new DataReader(socket.InputStream);
        reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
        reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
        reader.InputStreamOptions = InputStreamOptions.Partial;
        var count = await reader.LoadAsync(4);
        avail = reader.ReadString(count);
    }

    async private Task waitAvail()
    {
        while (avail != "TRUE")
        {
            await recieve();
        }
    }
#endif

}
