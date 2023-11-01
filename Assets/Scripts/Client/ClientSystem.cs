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
    //싱글톤
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

    private bool isAccepting;                                                   //연결중인지 확인하기 위한 변수
    private bool isConnected;                                                   //연결되었는지 확인하기 위한 변수

    private float acceptWaitTimer;
    private float disconnectTimer;

    private byte[] buffer;
    private const int max_BufferSize = 1024;

    [SerializeField] private TMP_InputField name_InputField;
    [SerializeField] private TMP_InputField ip_InputField;
    [SerializeField] private TMP_InputField port_InputField;

    [SerializeField] private float acceptWaitTime;                              //서버와 연결이 될 때까지 기다리는 시간
    [SerializeField] private float disconnectTime;                              //서버와 연결이 끊겼을 때 그 연결을 완전히 끊을 때까지 기다리는 시간

    private void Awake()
    {
        //싱글톤
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

        //UDP 소켓 생성 (Non-Blocking)
        clntSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        clntSock.Blocking = false;

        isAccepting = false;
        isConnected = false;
        acceptWaitTimer = 0;
        disconnectTimer = 0;

        //버퍼 초기화
        buffer = new byte[max_BufferSize];
    }
    private void Update()
    {
        //더미 EndPoint (서버 외의 다른 EndPoint를 받을 필요는 없으므로 사용하지 않는다)
        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

        //서버와 연결이 되어 있는 경우
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
                        // Ping에 1을 더한 후 string으로 변환, 그리고 앞에 PacketType을 추가하고 서버에게 SendTo를 한다
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
                        GAME : 인게임 통신 프로토콜
                         1. 여기에서는 서버로부터 받은 데이터를 해석하고 인게임에 적용하며 실제 전송은 SendToServer 메소드에서 이루어진다
                         2. 서버로부터 문자열 데이터를 수신받는데 그 문자열은 항목과 그 항목을 분류하기 위한 '/'로 이루어져 있다 (Ex. "Yugi/0.1/0.5")
                         3. 그러므로 받을 때 '/'를 기준으로 문자열을 분할한 후 순서에 따라 해석을 시작한다
                         4. 해석 순서는 버전에 따라 달라질 수 있으며 현재는 "이름/X_Pos/Y_Pos" 순서이다 (이름의 경우 모든 클라이언트 상에서 절대 중복되면 안된다)


                         당장 플레이어가 움직일 수 있는 수준까지 만들었지만 수정해야할 부분이 몇 가지 존재
                         1. NonPlayer Prefab외에는 생성이 불가능
                         2. 이름이 서로 다른 경우
                         3. 본인을 동기화하지 않음 (서버와 위치 오차가 생길 확률이 농후함)

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
                서버와 연결이 끊어진 경우
                1. TitleScene으로 돌아간다

                */

                Debug.Log("Disconnected");
                isConnected = false;

                SceneManager.LoadScene(0);
            }
        }

        //서버와 연결이 되지 않은 경우
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
        ArrangePacket : byte인 버퍼를 string으로 인코딩 후 사용되는 부분만 잘라서 반환 (PacketType 부분도 같이 자른다)

        */

        string arrangedBuffer = Encoding.UTF8.GetString(buffer);
        arrangedBuffer = arrangedBuffer.Substring(0, arrangedBuffer.IndexOf('\0'));
        arrangedBuffer = arrangedBuffer.Substring(2);
        return arrangedBuffer;
    }

    private PacketType GetPacketType(byte[] buffer)
    {
        /*
        GetPacketType : 서버로부터 받은 버퍼의 앞 두 글자(패킷의 용도)를 받아 PacketType으로 반환 

        */

        string bufferToString = Encoding.UTF8.GetString(buffer);
        bufferToString = bufferToString.Substring(0, 2);
        return (PacketType)System.Convert.ToInt32(bufferToString);
    }

    private string SetPacketType(string original, PacketType type)
    {
        /*
        SetPacketType : 패킷 앞에 어떤 용도로 사용되었는지 16진수로 명시하는 작업을 실시 (용도의 종류는 PacketType 참조)
         1. type를 16진수 변환 후 문자열로 typeString에 저장
         2. typeString + 원본 데이터 순서로 문자열 재구축 후 반환 (Ex. 011452 = 핑으로 사용되는 네자리 난수)
        */

        string typeString = System.Convert.ToString((int)type, 16);
        typeString = typeString.PadLeft(2, '0');
        original = typeString + original;
        return original;
    }

    public void ConnectToServer()
    {
        /*
        ConnectToServer : 버튼 UI로 호출되며 다음과 같이 작동한다
         1. servEP에 inputfield에 저장된 IP/Port를 초기화
         2. 용도(Accept) + name을 서버에게 전송
         3. 서버로부터 에코를 받은 경우 연결 완료 (isConnected = true)
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
                //임시로 isConnected 시키는 문장 (ReceiveFrom이 싪패했을 때 대처 사항이 아직 없음)
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
