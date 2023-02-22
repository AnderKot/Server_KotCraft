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
using System.Net.Security;
using BaseObjects;
using UnityEditor.PackageManager;
using MyStruct;

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
            Debug.Log("UDP-отправитель запущен...");

            try
            {
                if (MyClient == null)
                {
                    foreach (Packet Outpacket in OutPackets)
                    {
                        OutUdpSocket.ReceiveBufferSize = Outpacket.GetDataLength();
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

        public delegate void OutCallback(Packet InPacket);
        public OutCallback ReturnPacket;

        public UDPObserver(EndPoint inPoint, OutCallback returnPacket)
        {
            InPoint = inPoint;
            ReturnPacket = returnPacket;
        }

        public void ReseiveLoop()
        {
            //IPEndPoint localIP = new IPEndPoint(IPAddress.Parse(IP), Port);
            OutUdpSocket.Bind(InPoint);
            Debug.Log("UDP-слушатель запущен..." + InPoint.ToString());
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
                        ReturnPacket(new Packet(ResultStatus.RemoteEndPoint, data));
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
        public Packet(Packet packet)
        {
            this.Data.AddRange(packet.Data);
            Point = packet.Point;
        }

        public Packet(EndPoint point, byte[] byteData)
        {
            Point = point;
            Data.AddRange(byteData);
        }

        // -- типизированные сообщения
        public Packet(EndPoint point, int number) // - Передает число
        {
            Point = point;
            Data.Add(1);
            Data.Add((byte) number);
        }

        public Packet(EndPoint point, string message) // - Передает строку
        {
            Point = point;
            Data.Add(2);
            Data.AddRange(Encoding.UTF8.GetBytes(message));
        }

        public Packet(EndPoint point, Transform transfom, int netID) // - Передает положение объекта
        {
            Point = point;

            MemoryStream Stream = new MemoryStream();
            BinaryWriter Writer = new BinaryWriter(Stream);
            Writer.Write(netID);
            Writer.Write(transfom.position.x);
            Writer.Write(transfom.position.y);
            Writer.Write(transfom.position.z);

            Writer.Write(transfom.rotation.x);
            Writer.Write(transfom.rotation.y);
            Writer.Write(transfom.rotation.z);
            Writer.Write(transfom.rotation.w);


            Data.Add(3);
            Data.AddRange(Stream.ToArray());
        }

        public Packet(EndPoint point, Chunk chank)
        {
            Point = point;

            MemoryStream Stream = new MemoryStream();
            BinaryWriter Writer = new BinaryWriter(Stream);

            Writer.Write((int)chank.ChankPoint.x);
            Writer.Write((int)chank.ChankPoint.z);

            Writer.Write((int)chank.Blocks.Count);

            foreach (KeyValuePair<BlockPos, int> block in chank.Blocks)
            {
                Writer.Write((int)block.Key.x);
                Writer.Write((int)block.Key.y);
                Writer.Write((int)block.Key.z);
                Writer.Write((int)block.Value);
            }

            Data.Add(4);
            Data.AddRange(Stream.ToArray());
        }

        // -- Сбор данных на отправку
        public byte[] GetData()
        {
            return Data.ToArray();
        }

        public int GetDataLength()
        {
            return Data.Count;
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
            return Data.GetRange(2, Data.Count).ToArray().ConvertTo<Int32>();
        }
    }

    public class PaketsClientsMultiplaer
    {
        public byte[] Data;
        private List<EndPoint> ClientPoints = new List<EndPoint>();

        public PaketsClientsMultiplaer(List<EndPoint> clientPoints, byte[] data)
        {
            Data = data;
            ClientPoints.AddRange(clientPoints);
        }

        public void MultiplaerLoop()
        {
            Debug.Log("Мультипликатор пакетов запущен...");

            try
            {
                foreach (EndPoint point in ClientPoints)
                {
                    MyNETServer.OutAlertPackets.Add(new Packet(point, Data));
                }

            }
            catch (ThreadAbortException)
            {
                Debug.Log("Мультипликатор пакетов выходит из цикла");
            }
            Debug.Log("Мультипликатор пакетов закончил");
        }
    }
}
