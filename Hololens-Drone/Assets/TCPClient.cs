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

            client.Connect("152.23.202.92", 5717);
            // use the ipaddress as in the server program

            Debug.Log("Connected");

            String str = "some commands";
            String str2 = "test1";
            String str3 = "test3";
            String quitstr = "QUIT";
            Stream stm = client.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();

            byte[] ba = asen.GetBytes(str);
            Debug.Log("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            ba = asen.GetBytes(str2);
            stm.Write(ba, 0, ba.Length);

            ba = asen.GetBytes(str3);
            stm.Write(ba, 0, ba.Length);

            ba = asen.GetBytes(quitstr);
            stm.Write(ba, 0, ba.Length);

            Debug.Log("Messege sent, closing client");
            client.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Error..... " + e.StackTrace);
        }
    }
}
