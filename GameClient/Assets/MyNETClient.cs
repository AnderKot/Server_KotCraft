using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

using MyNET;


public class MyNETClient : MonoBehaviour
{
    public string IP;
    public int OutPort;
    public int InPort;
    public float PingDelayStep;
    public bool IsConected;

    
    // Для слушателя
    private UDPObserver Observer;
    private Thread ObserverThread;
    private List<Packet> InPackets = new List<Packet>();
    private EndPoint ObserverPoint;

    // Для отправки
    private UDPSender Sender;
    private Thread SenderThread;
    private List<Packet> OutPackets = new List<Packet>();
    private float PingDelay = 1;

    // Start is called before the first frame update
    void Start()
    {
        IsConected = false;

        // настройка слушателя
        Observer = new UDPObserver();
        Observer.IP = IP;
        Observer.Port = InPort;
        Observer.InPort = GetServerPacket;
        ObserverThread = new Thread(new ThreadStart(Observer.ReseiveLoop));
        ObserverThread.IsBackground = true;
        ObserverThread.Start();

        // Настройка отправки
        ObserverPoint = new IPEndPoint(IPAddress.Parse(IP), OutPort);
        Sender = new UDPSender();
        SenderThread = new Thread(new ThreadStart(Sender.SendLoop));
        SenderThread.IsBackground = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsConected)
        {
            foreach (Packet packet in InPackets)
            {
                switch (packet.GetPacketType())
                {
                    case 0:
                        break;
                    case 1:
                        Debug.Log("Получил (" + packet.GetString() + ")");
                        break;
                    case 2:
                        break;
                }
            }
        }
        else
        {
            if (PingDelay <= 0)
            {
                if (!SenderThread.IsAlive)
                {
                    //ClientThread.Abort();
                    Debug.Log("Пинг сервака на подключение");
                    Sender.OutPackets.Add(new Packet(ObserverPoint, "Hi ?"));
                    SenderThread = new Thread(new ThreadStart(Sender.SendLoop));
                    SenderThread.Start();
                    PingDelay = 1;
                }
            }
            PingDelay -= PingDelayStep;

            foreach (Packet packet in InPackets)
            {
                if (packet.GetPacketType() == 1)
                {
                    if (packet.GetString() == "Hi !")
                    {
                        Debug.Log("Получил подтверждение на подключение");
                        IsConected = true;
                    }
                    else
                    {
                        Debug.Log("Получил <<");
                        Debug.Log(packet.GetString());
                        Debug.Log("Получил >>");
                    }
                }

            }
        }

        InPackets.Clear();
    }

    void OnDestroy()
    {
        if (SenderThread.IsAlive)
        {
            Debug.Log("Пытался стопнуть UDP-отправку клиента");
            SenderThread.Abort();
            SenderThread.Join();
            Debug.Log("Cтопнул UDP-отправку клиента");
        }

        if (ObserverThread.IsAlive)
        {
            Debug.Log("Пытался стопнуть UDP-Слушателя клиента");
            ObserverThread.Abort();
            ObserverThread.Join();
            Debug.Log("Cтопнул UDP-Слушателя  клиента");
        }
    }

    public void Runing()
    {
        if (!SenderThread.IsAlive)
        {
            //ClientThread.Abort();
            Sender.OutPackets.Add(new Packet(ObserverPoint, "Привет давай данные"));
            SenderThread = new Thread(new ThreadStart(Sender.SendLoop));
            SenderThread.Start();
        }
    }

    public void GetServerPacket(Packet InPacket)
    {
        InPackets.Add(InPacket);
        Debug.Log("Принял от сервера ответ (" + InPacket.Point + ")");
    }
}
