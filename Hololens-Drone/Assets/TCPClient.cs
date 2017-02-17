using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using UnityEngine;

public class TCPClient : MonoBehaviour {
    public TcpClient client;
	// Use this for initialization
	void Start () {
        if (client == null)
        {
            Debug.Log("Starting Client...");
            main();
        }
    }

	
	// Update is called once per frame
	void Update () {

	}

    void main() {
        try
        {
            client = new TcpClient();
            Debug.Log("Connecting.....");

            client.Connect("152.23.119.139", 5717);
            // use the ipaddress as in the server program

            Debug.Log("Connected");

            String str = "some commands";
            Stream stm = client.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();

            byte[] ba = asen.GetBytes(str);
            Debug.Log("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);

            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(bb[i]));
            Debug.Log("Messege sent, closing client");
            client.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Error..... " + e.StackTrace);
        }
    }
}
