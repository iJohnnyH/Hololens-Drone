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

            client.Connect("152.23.1.224", 5717);
            //Use RPi ip address

            Debug.Log("Connected");

            String str = "1";
            String str2 = "2";
            String str3 = "3";
            String str4 = "4";
            String str5 = "5";
            String quitstr = "QUIT";
            Stream stm = client.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();

            byte[] strData = asen.GetBytes(str);
            Debug.Log("Writing 1");
            stm.Write(strData, 0, strData.Length);
            System.Threading.Thread.Sleep(5);

            Debug.Log("Writing 2");
            strData = asen.GetBytes(str2);
            stm.Write(strData, 0, strData.Length);
            System.Threading.Thread.Sleep(5);

            Debug.Log("Writing 3");
            strData = asen.GetBytes(str3);
            stm.Write(strData, 0, strData.Length);
            System.Threading.Thread.Sleep(5);

            Debug.Log("Writing 4");
            strData = asen.GetBytes(str4);
            stm.Write(strData, 0, strData.Length);
            System.Threading.Thread.Sleep(5);

            Debug.Log("writing 5");
            strData = asen.GetBytes(str5);
            stm.Write(strData, 0, strData.Length);
            System.Threading.Thread.Sleep(5);

            Debug.Log("Writing QUIT");
            strData = asen.GetBytes(quitstr);
            stm.Write(strData, 0, strData.Length);
            System.Threading.Thread.Sleep(5);

            Debug.Log("Messege(s) sent, closing client");
            client.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Error..... " + e.StackTrace);
        }
    }
}
