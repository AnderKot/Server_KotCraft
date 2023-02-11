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
using UnityEngine.UIElements;
using BaseObjects;

namespace MyNET
{
    public class UDPSender
    {
        private Socket OutUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public List<Packet> OutPackets = new List<Packet>();
        public EndPoint MyClient;

        public ExampleCallback callback;

        public void SendLoop()
        {

            EndPoint remoteOutPoint; // адрес куда посылаем

            Debug.Log("UDP-отправитель запущен...");

            try
            {
                if (MyClient == null)
                {
                    foreach (Packet Outpacket in OutPackets)
                    {
                        Task<int> ResultTask = OutUdpSocket.SendToAsync(Outpacket.GetData(), SocketFlags.None, Outpacket.Point);
                        int R = ResultTask.Result;
                        //Debug.Log("UDP-отправитель послал: " + Outpacket.Message + " в " + remoteOutPoint.ToString());
                        Thread.Sleep(0);
                    }
                }
                else 
                {
                    foreach (Packet Outpacket in OutPackets)
                    {
                        Task<int> ResultTask = OutUdpSocket.SendToAsync(Outpacket.GetData(), SocketFlags.None, MyClient);
                        int R = ResultTask.Result;
                        //Debug.Log("UDP-отправитель послал: " + Outpacket.Message + " в " + remoteOutPoint.ToString());
                        Thread.Sleep(0);
                    }
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
        byte[] data; // Битовый буфер
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
                    data = new byte[OutUdpSocket.ReceiveBufferSize];
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
            Data.Add(1);
            Data.Add((byte)Type);
        }

        public Packet(EndPoint point, string message)
        {
            Point = point;
            Data.Add(2);
            Data.AddRange(Encoding.UTF8.GetBytes(message));
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
            return Encoding.UTF8.GetString(Data.GetRange(1, Data.Count - 1).ToArray(), 0, Data.Count);
        }

        public int GetInt()
        {
            return Data.GetRange(1, Data.Count - 1).ToArray().ConvertTo<Int32>();
        }

        public void GetTransform(ref Vector3 position,ref Quaternion rotation)
        {
            Data.RemoveAt(0);
            MemoryStream Stream = new MemoryStream(Data.ToArray());
            BinaryReader Reader = new BinaryReader(Stream);

            position = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            rotation = new Quaternion(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());

        }

        public Chank GetChank()
        {
            Data.RemoveAt(0);
            MemoryStream Stream = new MemoryStream(Data.ToArray());
            BinaryReader Reader = new BinaryReader(Stream);

            Vector3Int ChankPosition = new Vector3Int(Reader.ReadInt32(), 0, Reader.ReadInt32());

            int BloksCount = Reader.ReadInt32();
            Dictionary<Vector3Int, int> BlocsID = new Dictionary<Vector3Int, int>();

            for (int i = 0; i < BloksCount; i++)
            {
                BlocsID.Add(new Vector3Int(Reader.ReadInt32(), Reader.ReadInt32(), Reader.ReadInt32()), Reader.ReadInt32());
            }

            return new Chank(ChankPosition, BlocsID);

        }


    }

}
