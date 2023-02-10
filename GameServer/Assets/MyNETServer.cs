using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

using MyNET;
using System.Net.Http;
using UnityEngine.UIElements;
using System.Security;
using UnityEditor.PackageManager;
using BaseCreature;

public class MyNETServer : MonoBehaviour
{
    public string IP;
    public int InPort;
    public int OutPort;
    public string Message;
    public float PingDelayStep = 0.01f;

    // Для слушателя
    private UDPObserver Observer;
    private Thread ObserverTread;
    private List<Packet> InPackets = new List<Packet>();

    // Для отправки
    private static List<Packet> OutAlertPackets = new List<Packet>();
    public  static List<Packet> OutPackets = new List<Packet>();
    private float PingDelay = 1;

    private List<EndPoint> ClientPoints = new List<EndPoint>();
    private List<Player> Players = new List<Player>(); 
    public bool IsAddTestClient;
    public int ClientsCount = 0;

    //
    public List<Transform> ObservesObjects = new List<Transform>();

    void Start()
    {
        // настройка слушателя
        Observer = new UDPObserver();
        Observer.IP = IP;
        Observer.Port = InPort;
        Observer.InPort = GetServerPacket;
        ObserverTread = new Thread(new ThreadStart(Observer.ReseiveLoop));
        ObserverTread.IsBackground = true;
        ObserverTread.Start();

    }

    void FixedUpdate()
    {
        if(IsAddTestClient)
        {
            IsAddTestClient = false;
            InPackets.Add(new Packet(new IPEndPoint(IPAddress.Parse(IP+ClientsCount.ToString()), InPort), 0));
        }

        // Разбор пришедших пакетов
        foreach (Packet packet in InPackets)
        {
            string PointString = packet.Point.ToString();
            int SeparanotIndex = PointString.IndexOf(':');
            string ClientIP = PointString.Remove(SeparanotIndex, PointString.Length - SeparanotIndex);
            EndPoint ClientPoint = new IPEndPoint(IPAddress.Parse(ClientIP), OutPort);



            switch (packet.GetPacketType())
            {
                case 1:
                    if (! ClientPoints.Contains(ClientPoint))
                    {
                        ClientPoints.Add(ClientPoint);
                        Players.Add(new Player(ClientIP, OutPort));
                        ClientsCount++;
                        OutAlertPackets.Add(new Packet(ClientPoint, ClientsCount));
                        Debug.Log("Принял нового клиента:("+ ClientPoint + ")");
                    }
                    break;
                case 2:
                    Message = packet.GetString();
                    break;
                case 3:
                    break;
            }

            Debug.Log("Обработанно пакетов (" + InPackets.Count + ")");
        }
        InPackets.Clear();

        // Отправка срочных пакетов
        if (OutAlertPackets.Count > 0)
        {
            foreach (EndPoint client in ClientPoints)
            {
                // Настройка отправки
                UDPSender Sender;
                Thread SenderThread;

                Sender = new UDPSender();
                Sender.OutPackets.AddRange(OutAlertPackets);
                SenderThread = new Thread(new ThreadStart(Sender.SendLoop));
                SenderThread.IsBackground = true;
                SenderThread.Start();
                Debug.Log("Срочная UDP-Отправка клиентам (" + ClientPoints.Count + ") пакетов (" + OutAlertPackets.Count + ")");


            }
            OutAlertPackets.Clear();
        }
        
        // Отправка не срочных пакетов
        if (PingDelay <= 0)
        {
            if (OutPackets.Count > 0)
            {
                foreach (EndPoint client in ClientPoints)
                {
                    // Настройка отправки
                    UDPSender Sender;
                    Thread SenderThread;

                    Sender = new UDPSender();
                    Sender.OutPackets.AddRange(OutPackets);
                    SenderThread = new Thread(new ThreadStart(Sender.SendLoop));
                    SenderThread.IsBackground = true;
                    SenderThread.Start();
                    Debug.Log("Обычная UDP-Отправка клиентам (" + ClientPoints.Count + ") пакетов (" + OutPackets.Count + ")");


                }
            }
            PingDelay = 1;
        }
        PingDelay -= PingDelayStep;
        OutPackets.Clear();

        }

    void OnDestroy()
    {
        if (ObserverTread.IsAlive)
        {
            Debug.Log("Пытался стопнуть UDP-Слушателя сервера");
            ObserverTread.Abort();
            ObserverTread.Join();
            Debug.Log("Cтопнул UDP-Слушателя сервера");
        }
    }


    public void GetServerPacket(Packet InPacket)
    {
        InPackets.Add(InPacket);
        Debug.Log("Принял от клиента пакет (" + InPacket.Point + ")"); 
    }




    public void Runing()
    {
       
    }

}