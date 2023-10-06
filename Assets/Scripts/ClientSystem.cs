using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ClientSystem : MonoBehaviour
{
    private Socket clntSock;

    private void Start()
    {
        clntSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    private void SendToServer(InputValue input)
    {
 


    }
}
