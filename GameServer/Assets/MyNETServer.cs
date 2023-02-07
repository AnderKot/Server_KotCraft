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
    private UDPSender Sender;
    private Thread SenderThread;
    private List<Packet> OutPackets = new List<Packet>();
    private float PingDelay = 1;

    private List<EndPoint> Clients = new List<EndPoint>();
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

        // Настройка отправки
        Sender = new UDPSender();
        SenderThread = new Thread(new ThreadStart(Sender.SendLoop));
        SenderThread.IsBackground = true;
        
}

    void FixedUpdate()
    {
        
        // Разбор пришедших пакетов
        foreach(Packet packet in InPackets)
        {
            string PointString = packet.Point.ToString();
            int SeparanotIndex = PointString.IndexOf(':');
            string ClientIP = PointString.Remove(SeparanotIndex, PointString.Length - SeparanotIndex);
            EndPoint ClientPoint = new IPEndPoint(IPAddress.Parse(ClientIP), OutPort);

            if (! Clients.Contains(ClientPoint))
            {
                Clients.Add(ClientPoint);
                ClientsCount++;
                
                OutPackets.Add(new Packet(ClientPoint, "Hi !"));
                Debug.Log("UDP-Слушатель принял нового клиента:("+ ClientPoint + ")");
            }

            switch (packet.GetPacketType())
            {
                case 0:
                    break;
                case 1:
                    Message = packet.GetString();
                    break;
                case 2:
                    break;
            }
        }

        if (PingDelay <= 0)
        {
            // Сборка пакетов на отправку
            foreach (Transform ObservesObject in ObservesObjects)
            {
                TransformNETForm CurrTransformNET = new TransformNETForm(0, ObservesObject.position, ObservesObject.rotation);
                // Для каждого клиента
                foreach (EndPoint client in Clients)
                {
                    OutPackets.Add(new Packet(client, CurrTransformNET));
                }
            }
            PingDelay = 1;
        }
        PingDelay -= PingDelayStep;

        // Отправка пакетов
        if (!SenderThread.IsAlive & (OutPackets.Count > 0))
        {
            Sender.OutPackets.AddRange(OutPackets);
            SenderThread = new Thread(new ThreadStart(Sender.SendLoop));
            SenderThread.IsBackground = true;
            SenderThread.Start();
            Debug.Log("UDP-Отправка клиентам (" + Clients.Count + ") пакетов (" + OutPackets.Count + ")");
            OutPackets.Clear();
        }

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
        Debug.Log("Принял от клиента обработку (" + InPacket.Point + ")"); 
    }

    public void Runing()
    {
       
    }

}