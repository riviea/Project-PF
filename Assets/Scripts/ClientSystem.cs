using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;

public class ClientSystem : MonoBehaviour
{
    private Socket clntSock;

    public TMP_InputField ip_InputField;
    public TMP_InputField port_InputField;

    private void Start()
    {
        clntSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    public void SendToServer(InputValue input)
    {
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
    }
}
