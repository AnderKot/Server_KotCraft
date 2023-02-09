using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using BaseCreature;

namespace MyNET
{
    public class UDPSender
    {
        private Socket OutUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        byte[] data = new byte[512];

        public List<Packet> OutPackets = new List<Packet>();


        public ExampleCallback callback;

        public void SendLoop()
        {

            EndPoint remoteOutPoint; // адрес куда посылаем

            Debug.Log("UDP-отправитель запущен...");

            try
            {
                foreach (Packet Outpacket in OutPackets)
                {
                    remoteOutPoint = Outpacket.Point;
                    Task<int> ResultTask = OutUdpSocket.SendToAsync(Outpacket.GetData(), SocketFlags.None, remoteOutPoint);
                    int R = ResultTask.Result;
                    //Debug.Log("UDP-отправитель послал: " + Outpacket.Message + " в " + remoteOutPoint.ToString());
                    Thread.Sleep(0);
                }
            }
            catch (ThreadAbortException)
            {
                OutPackets.Clear();
                Debug.Log("UDP-отправитель выходит из цикла");
            }
            OutPackets.Clear();
            Debug.Log("UDP-отправитель закончил");
        }

        public delegate void ExampleCallback(string message);
    }



    public class UDPObserver
    {

        EndPoint InPoint = new IPEndPoint(IPAddress.Any, 0); // адрес откуда постучаем
        Socket OutUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        byte[] data = new byte[512]; // Битовый буфер
        public string IP;
        public int Port;

        public delegate void InCallback(Packet InPacket);
        public InCallback InPort;

        public void ReseiveLoop()
        {
            IPEndPoint localIP = new IPEndPoint(IPAddress.Parse(IP), Port);
            OutUdpSocket.Bind(localIP);
            Debug.Log("UDP-слушатель запущен..." + localIP.ToString());
            bool Run = true;

            while (Run)
            {
                try
                {
                    Task<SocketReceiveFromResult> ResultTask = OutUdpSocket.ReceiveFromAsync(data, SocketFlags.None, InPoint);
                    SocketReceiveFromResult ResultStatus = ResultTask.Result;
                    if (ResultStatus.ReceivedBytes > 0)
                    {
                        InPort(new Packet(ResultStatus.RemoteEndPoint, data));
                        //Debug.Log("UDP-сервер получил: " + message + " Из (" + ResultStatus.RemoteEndPoint.ToString() + ")");
                    }
                    else
                    {
                        Debug.Log("UDP-слушатель получил пустышку");
                    }
                    Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    Run = false;
                    Debug.Log("UDP-слушатель остановлен");
                }
            }
        }
    }

    public class Packet
    {
        public EndPoint Point;
        public List<byte> Data = new List<byte>();
        // -- Создание из пришедших данных
        public Packet(EndPoint point, byte[] byteData)
        {
            Point = point;
            Data.AddRange(byteData);
        }

        // -- типизированные сообщения
        public Packet(EndPoint point, int Type)
        {
            Point = point;
            Data.Add((byte)Type);
        }

        public Packet(EndPoint point, string message)
        {
            Point = point;
            Data.Add(1);
            Data.AddRange(Encoding.UTF8.GetBytes(message));
        }

        public Packet(EndPoint point, TransformNETForm transfonNET)
        {
            Point = point;

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream streamReader = new MemoryStream();
            formatter.Serialize(streamReader, transfonNET);
            byte[] data = new byte[streamReader.Length];
            streamReader.Read(data, 0, data.Length);

            Data.Add(2);
            Data.AddRange(data);
        }

        public Packet(EndPoint point, Transform transfom)
        {
            Point = point;
            TransformNETForm transfomNET = new TransformNETForm(0,transfom.position, transfom.rotation);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream streamReader = new MemoryStream();
            formatter.Serialize(streamReader, transfomNET);
            byte[] data = new byte[streamReader.Length];
            streamReader.Read(data, 0, data.Length);

            Data.Add(2);
            Data.AddRange(data);
        }

        // -- Сбор данных на отправку
        public byte[] GetData()
        {
            return Data.ToArray();
        }

        // -- Взять сообщение по типу
        public int GetPacketType()
        {
            return (int)Data[0];
        }

        public string GetString()
        {
            return Encoding.UTF8.GetString(Data.GetRange(2, Data.Count).ToArray(), 0, Data.Count);
        }


    }

    //[Serializable]

    [Serializable]
    public class TransformNETForm
    {
        //public int NETID { get; set; }

        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public float RotationW { get; set; }

        public TransformNETForm(int netID, Vector3 position, Quaternion rotation)
        {
            //NETID = NETID;

            PositionX = position.x;
            PositionY = position.y;
            PositionZ = position.z;

            RotationX = rotation.x;
            RotationY = rotation.y;
            RotationZ = rotation.z;
            RotationW = rotation.w;
        }

        public Vector3 GetPosition()
        {
            return (new Vector3(PositionX, PositionY, PositionZ));
        }
        public Quaternion GetRotation()
        {
            return (new Quaternion(RotationX, RotationY, RotationZ, RotationW));
        }
    }
    /*
    public class Client
    {
        IP ip;
        Player MyPlayer;


        public Client(string ip)
        {
            IP = ip;
            MyPlayer = new Player();
        }
    }
    */
}
