using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class ClientSystem : MonoBehaviour
{
    private Socket clntSock;
    private EndPoint servEP;

    private byte[] buf;
    private const int BUFSIZE = 1024;

    private bool isConnected = false;

    private GameObject playerPrefap;

    public TMP_InputField ip_InputField;
    public TMP_InputField port_InputField;

    private void Start()
    {
        clntSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        buf = new byte[BUFSIZE];
        playerPrefap = Resources.Load<GameObject>("Prefaps/Player");
    }

    private void FixedUpdate()
    {

    }

    public void ConnectedToServer()
    {
        if (ip_InputField.text != null && port_InputField.text != null)
        {
            servEP = new IPEndPoint(IPAddress.Parse(ip_InputField.text), int.Parse(port_InputField.text));

            int ack = UnityEngine.Random.Range(1000, 9998);
            byte[] ackToByte = Encoding.UTF8.GetBytes(ack.ToString());
            clntSock.SendTo(ackToByte, servEP);

            //Dummy EndPoint to use ReceiveFrom
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            clntSock.ReceiveFrom(buf, SocketFlags.None, ref endPoint);

            if(int.Parse(Encoding.UTF8.GetString(buf)) == ack + 1)
            {
                isConnected = true;
                Debug.Log("Sucessfully connected to Server");

                Instantiate(playerPrefap);
            }
        }
    }

    public void SendToServer(InputValue input)
    {
        /*
        byte[] buf = new byte[1024];
        Vector2 vec = input.Get<Vector2>();

        if (vec == Vector2.left)
            buf = Encoding.UTF8.GetBytes("A");
        if (vec == Vector2.right)
            buf = Encoding.UTF8.GetBytes("D");
        if (vec == Vector2.up)
            buf = Encoding.UTF8.GetBytes("W");
        if (vec == Vector2.down)
            buf = Encoding.UTF8.GetBytes("S");

        clntSock.SendTo(buf, new IPEndPoint(IPAddress.Parse(ip_InputField.text), int.Parse(port_InputField.text)));
        */
    }
}

