using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.NetworkInformation;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class ClientSystem : MonoBehaviour
{
    //�̱���
    public static ClientSystem clientSystem = null;

    public enum PacketType
    {
        ACCEPT = 1,
        PING = 2,
        MESSAGE = 3,
        GAME = 4
    }

    public enum GameObjectType
    {
        PLAYER = 1,
        AI = 2,
        PROJECTILE = 3
    }


    private Socket clntSock;
    private EndPoint servEP;
    public string playerName { get; private set; }

    private bool isAccepting;                                                   //���������� Ȯ���ϱ� ���� ����
    private bool isConnected;                                                   //����Ǿ����� Ȯ���ϱ� ���� ����

    private float acceptWaitTimer;
    private float disconnectTimer;

    private byte[] buffer;
    private const int max_BufferSize = 1024;

    [SerializeField] private TMP_InputField name_InputField;
    [SerializeField] private TMP_InputField ip_InputField;
    [SerializeField] private TMP_InputField port_InputField;

    [SerializeField] private float acceptWaitTime;                              //������ ������ �� ������ ��ٸ��� �ð�
    [SerializeField] private float disconnectTime;                              //������ ������ ������ �� �� ������ ������ ���� ������ ��ٸ��� �ð�

    private void Awake()
    {
        //�̱���
        if (clientSystem == null)
        {
            clientSystem = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (clientSystem != this)
                Destroy(this.gameObject);
        }

        //UDP ���� ���� (Non-Blocking)
        clntSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        clntSock.Blocking = false;

        isAccepting = false;
        isConnected = false;
        acceptWaitTimer = 0;
        disconnectTimer = 0;

        //���� �ʱ�ȭ
        buffer = new byte[max_BufferSize];
    }
    private void Update()
    {
        //���� EndPoint (���� ���� �ٸ� EndPoint�� ���� �ʿ�� �����Ƿ� ������� �ʴ´�)
        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

        //������ ������ �Ǿ� �ִ� ���
        if (isConnected)
        {
            try
            {
                clntSock.ReceiveFrom(buffer, SocketFlags.None, ref endPoint);
                disconnectTimer = 0.0f;

                string arrangedBuffer = ArrangeBuffer(buffer);

                switch (GetPacketType(buffer))
                {
                    case PacketType.ACCEPT:
                        break;

                    case PacketType.PING:
                        // Ping�� 1�� ���� �� string���� ��ȯ, �׸��� �տ� PacketType�� �߰��ϰ� �������� SendTo�� �Ѵ�
                        int ping = int.Parse(arrangedBuffer);
                        ping++;
                        arrangedBuffer = ping.ToString();
                        arrangedBuffer = SetPacketType(arrangedBuffer, PacketType.PING);
                        byte[] dataToBytes = Encoding.UTF8.GetBytes(arrangedBuffer);
                        clntSock.SendTo(dataToBytes, servEP);
                        break;

                    case PacketType.MESSAGE:
                        break;

                    case PacketType.GAME:

                        /*
                        GAME : �ΰ��� ��� ��������
                         1. ���⿡���� �����κ��� ���� �����͸� �ؼ��ϰ� �ΰ��ӿ� �����ϸ� ���� ������ SendToServer �޼ҵ忡�� �̷������
                         2. �����κ��� ���ڿ� �����͸� ���Ź޴µ� �� ���ڿ��� �׸�� �� �׸��� �з��ϱ� ���� '/'�� �̷���� �ִ� (Ex. "Yugi/0.1/0.5")
                         3. �׷��Ƿ� ���� �� '/'�� �������� ���ڿ��� ������ �� ������ ���� �ؼ��� �����Ѵ�
                         4. �ؼ� ������ ������ ���� �޶��� �� ������ ����� "�̸�/X_Pos/Y_Pos" �����̴� (�̸��� ��� ��� Ŭ���̾�Ʈ �󿡼� ���� �ߺ��Ǹ� �ȵȴ�)


                         ���� �÷��̾ ������ �� �ִ� ���ر��� ��������� �����ؾ��� �κ��� �� ���� ����
                         1. NonPlayer Prefab�ܿ��� ������ �Ұ���
                         2. �̸��� ���� �ٸ� ���
                         3. ������ ����ȭ���� ���� (������ ��ġ ������ ���� Ȯ���� ������)

                        */
                        string[] bufferSplited = arrangedBuffer.Split("~");

                        switch ((GameObjectType)int.Parse(bufferSplited[0]))
                        {
                            case GameObjectType.PLAYER:

                                if (bufferSplited[2] != playerName)
                                {
                                    GameObject gameObject = GameObject.Find(bufferSplited[2]);

                                    if (gameObject == null)
                                    {
                                        gameObject = Resources.Load("Prefabs/NonPlayer") as GameObject;
                                        gameObject = MonoBehaviour.Instantiate(gameObject);
                                        gameObject.name = bufferSplited[2];
                                    }
                                    gameObject.transform.localPosition = new Vector3(float.Parse(bufferSplited[3]), float.Parse(bufferSplited[4]), 0);
                                }

                                break;

                            case GameObjectType.AI:
                                break;

                            case GameObjectType.PROJECTILE:
                                //Ex. 3~Prefabs/Bullet~Yugi~0.1~0.1~0.2~0.2

                                if (bufferSplited[2] != playerName)
                                {
                                    Vector2 firePos;
                                    firePos.x = float.Parse(bufferSplited[3]);
                                    firePos.y = float.Parse(bufferSplited[4]);

                                    Vector2 mousePos;
                                    mousePos.x = float.Parse(bufferSplited[5]);
                                    mousePos.y = float.Parse(bufferSplited[6]);

                                    GameObject projectile = Resources.Load(bufferSplited[1]) as GameObject;
                                    projectile = MonoBehaviour.Instantiate(projectile);    

                                    Projectile bulletProjectile = projectile.GetComponent<Projectile>();

                                    bulletProjectile.Launch(firePos, mousePos);
                                }

                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                disconnectTimer += Time.deltaTime;
            }

            if (disconnectTimer > disconnectTime)
            {
                /*
                ������ ������ ������ ���
                1. TitleScene���� ���ư���

                */

                Debug.Log("Disconnected");
                isConnected = false;

                SceneManager.LoadScene(0);
            }
        }

        //������ ������ ���� ���� ���
        else if (!isConnected)
        {
            if (isAccepting)
            {
                try
                {
                    clntSock.ReceiveFrom(buffer, SocketFlags.None, ref endPoint);
                    string arrangedBuffer = ArrangeBuffer(buffer);

                    if (GetPacketType(buffer) == PacketType.ACCEPT)
                    {
                        if (arrangedBuffer == playerName)
                        {
                            isConnected = true;
                            isAccepting = false;

                            Debug.Log("Sucessfully connected to Server");

                            SceneManager.LoadScene(1);
                        }
                    }
                }
                catch (Exception e)
                {
                    acceptWaitTimer += Time.deltaTime;
                }

                if (acceptWaitTimer > acceptWaitTime)
                {
                    Debug.Log("Failed to connected to Server");
                    isAccepting = false;
                }
            }
        }
    }

    private string ArrangeBuffer(byte[] buffer)
    {
        /*
        ArrangePacket : byte�� ���۸� string���� ���ڵ� �� ���Ǵ� �κи� �߶� ��ȯ (PacketType �κе� ���� �ڸ���)

        */

        string arrangedBuffer = Encoding.UTF8.GetString(buffer);
        arrangedBuffer = arrangedBuffer.Substring(0, arrangedBuffer.IndexOf('\0'));
        arrangedBuffer = arrangedBuffer.Substring(2);
        return arrangedBuffer;
    }

    private PacketType GetPacketType(byte[] buffer)
    {
        /*
        GetPacketType : �����κ��� ���� ������ �� �� ����(��Ŷ�� �뵵)�� �޾� PacketType���� ��ȯ 

        */

        string bufferToString = Encoding.UTF8.GetString(buffer);
        bufferToString = bufferToString.Substring(0, 2);
        return (PacketType)System.Convert.ToInt32(bufferToString);
    }

    private string SetPacketType(string original, PacketType type)
    {
        /*
        SetPacketType : ��Ŷ �տ� � �뵵�� ���Ǿ����� 16������ ����ϴ� �۾��� �ǽ� (�뵵�� ������ PacketType ����)
         1. type�� 16���� ��ȯ �� ���ڿ��� typeString�� ����
         2. typeString + ���� ������ ������ ���ڿ� �籸�� �� ��ȯ (Ex. 011452 = ������ ���Ǵ� ���ڸ� ����)
        */

        string typeString = System.Convert.ToString((int)type, 16);
        typeString = typeString.PadLeft(2, '0');
        original = typeString + original;
        return original;
    }

    public void ConnectToServer()
    {
        /*
        ConnectToServer : ��ư UI�� ȣ��Ǹ� ������ ���� �۵��Ѵ�
         1. servEP�� inputfield�� ����� IP/Port�� �ʱ�ȭ
         2. �뵵(Accept) + name�� �������� ����
         3. �����κ��� ���ڸ� ���� ��� ���� �Ϸ� (isConnected = true)
        */

        if (!isConnected)
        {
            if (ip_InputField.text != null && port_InputField.text != null)
            {
                //1.
                servEP = new IPEndPoint(IPAddress.Parse(ip_InputField.text), int.Parse(port_InputField.text));

                //2.
                playerName = name_InputField.text;

                string name = name_InputField.text;
                name = SetPacketType(name, PacketType.ACCEPT);
                byte[] nameToBytes = Encoding.UTF8.GetBytes(name);
                clntSock.SendTo(nameToBytes, servEP);

                //3.
                //�ӽ÷� isConnected ��Ű�� ���� (ReceiveFrom�� �������� �� ��ó ������ ���� ����)
                isAccepting = true;
            }
        }
    }


    public void SendToServer(string data, PacketType packetType)
    {
        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

        data = SetPacketType(data, packetType);
        byte[] dataToBytes = Encoding.UTF8.GetBytes(data);
        clntSock.SendTo(dataToBytes, servEP);
    }
}
