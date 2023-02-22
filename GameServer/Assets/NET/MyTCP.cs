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
            List<byte> DataBuff = new List<byte>(); // �������� �����
            List<byte> TempDataBuff = new List<byte>();
            int ReceivedBytes;

            Debug.Log("TCP-����������� �������...");
            TCPSocket.Connect(OutPoint);
            Debug.Log("TCP-����������� ������ ����������");

            try
            {
                foreach (Packet Outpacket in OutPackets)
                {
                    DataBuff.Clear();
                    DataBuff.AddRange(Outpacket.GetData());

                    while (DataBuff.Count > 500)  // ����� �� ����� �� 500
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
                Debug.Log("TCP-����������� ������� �� �����");
            }
            OutPackets.Clear();
            Debug.Log("TCP-����������� ��������");
        }
    }

    public class TCPObserver
    {

        EndPoint InPoint; // ����� ������ ���������
        Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<byte> DataBuff; // �������� �����

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
            Debug.Log("TCP-��������� �������..." + InPoint.ToString());

            bool Run = true;

            Socket AcceptSoket = TCPSocket.Accept();
            byte[] data = new byte[501];
            Debug.Log("TCP-��������� ������ ���������� � " + AcceptSoket.RemoteEndPoint);

            while (Run)
            {
                try
                {
                    ReceivedBytes = AcceptSoket.Receive(data);
                    if (ReceivedBytes > 0)
                    {
                        if (data[500] == 1)// �������� �����
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
                        Debug.Log("TCP-��������� ������� ��������");
                    }
                    Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    Run = false;
                    AcceptSoket.Close();
                    TCPSocket.Close();
                    Debug.Log("TCP-��������� ����������");
                }
            }
        }
    }
    
}
