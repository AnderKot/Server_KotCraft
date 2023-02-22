using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MyNET
{
    public class TCPSender
    {
        private Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);



        public List<Packet> OutPackets = new List<Packet>();
        public EndPoint OutPoint;

        public TCPSender(EndPoint outPoint, List<Packet> outPackets)
        {
            OutPoint = outPoint;
            OutPackets.AddRange(outPackets);
        }

        public void SendLoop()
        {
            List<byte> DataBuff = new List<byte>(); // Байтовый буфер
            List<byte> TempDataBuff = new List<byte>();
            int ReceivedBytes;

            Debug.Log("TCP-отправитель запущен...");
            TCPSocket.Connect(OutPoint);
            Debug.Log("TCP-отправитель принял соединение");

            try
            {
                foreach (Packet Outpacket in OutPackets)
                {
                    DataBuff.Clear();
                    DataBuff.AddRange(Outpacket.GetData());

                    while (DataBuff.Count > 500)  // режем на пачки по 500
                    {
                        TempDataBuff.Clear();
                        TempDataBuff.AddRange(DataBuff.GetRange(0, 500));
                        DataBuff.RemoveRange(0, 500);
                        TempDataBuff.Add(1);

                        ReceivedBytes = TCPSocket.Send(TempDataBuff.ToArray());
                    }
                    DataBuff.Add(0);
                    ReceivedBytes = TCPSocket.Send(TempDataBuff.ToArray());
                }
                
            }
            catch (ThreadAbortException)
            {
                OutPackets.Clear();
                Debug.Log("TCP-отправитель выходит из цикла");
            }
            OutPackets.Clear();
            Debug.Log("TCP-отправитель закончил");
        }
    }

    public class TCPObserver
    {

        EndPoint InPoint; // адрес откуда постучаем
        Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<byte> DataBuff; // Байтовый буфер

        public delegate void OutCallback(Packet InPacket);
        public OutCallback ReturnPacket;

        public TCPObserver(EndPoint inPoint, OutCallback returnPacket)
        {
            InPoint = inPoint;
            ReturnPacket = returnPacket;
        }

        public void ReseiveLoop()
        {
            int ReceivedBytes;

            TCPSocket.Bind(InPoint);
            TCPSocket.Listen(1);
            Debug.Log("TCP-слушатель запущен..." + InPoint.ToString());

            bool Run = true;

            Socket AcceptSoket = TCPSocket.Accept();
            byte[] data = new byte[501];
            Debug.Log("TCP-слушатель принял соединение с " + AcceptSoket.RemoteEndPoint);

            while (Run)
            {
                try
                {
                    ReceivedBytes = AcceptSoket.Receive(data);
                    if (ReceivedBytes > 0)
                    {
                        if (data[500] == 1)// Собираем пачку
                        {
                            DataBuff.AddRange(data);
                            DataBuff.RemoveAt(DataBuff.Count);
                        }
                        else
                        {
                            DataBuff.AddRange(data);
                            //DataBuff.RemoveAt(DataBuff.FindLastIndex(DataBuff.Count - 500, lastByte => lastByte == 1));
                            ReturnPacket(new Packet(AcceptSoket.RemoteEndPoint, DataBuff.ToArray()));
                        }
                        
                        
                    }
                    else
                    {
                        Debug.Log("TCP-слушатель получил пустышку");
                    }
                    Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    Run = false;
                    AcceptSoket.Close();
                    TCPSocket.Close();
                    Debug.Log("TCP-слушатель остановлен");
                }
            }
        }
    }
    
}
