using System;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Threading;
using Workers;
using MyStruct;
using MyNET;
using System.Collections.Generic;
using System.Net;

namespace BaseCreature 
{
    public class Player
    {
        int NETID;
        string IP;
        string Name;
        
        public Vector3 MyPosition;
        GameObject MyObject;
        //ChankLoader ChankLoader;
        Thread CalcCurrChankThread;
        public ChunkPos CurrChank;

        // --Сетевая часть--
        EndPoint TCPOutPoint;
        EndPoint TCPInPoint;
        private List<Packet> TCPInPackets = new List<Packet>();
        private List<Packet> TCPOutPackets = new List<Packet>();
        private Thread TCPObserverTread;

        EndPoint UDPOutPoint;
        EndPoint UDPInPoint;
        private List<Packet> UDPInPackets = new List<Packet>();
        private List<Packet> UDPOutPackets = new List<Packet>();
        private Thread UDPObserverTread;

        // --Состояния--
        public bool IsSpawnedOnClient;

        private static GameObject PlayerGameObject = Resources.Load<GameObject>("Player");


        public Player(string ip, int portStep,int NETID)
        {
            IP = ip;
            GetStoreData(ip);
            // --Подгрузка местности--
            AreaChunkToFactoryAdder ChankAdder = new AreaChunkToFactoryAdder(CurrChank);
            Thread AreaFactoryAdderThread = new Thread(new ThreadStart(ChankAdder.AdderLoop));
            AreaFactoryAdderThread.IsBackground = true;
            AreaFactoryAdderThread.Start();
            // --настройка сетевой части
            TCPOutPoint = new IPEndPoint(IPAddress.Parse(IP), 65500);
            TCPInPoint = new IPEndPoint(IPAddress.Parse(IP), 65500 + portStep);

            UDPOutPoint = new IPEndPoint(IPAddress.Parse(IP), 65501);
            UDPInPoint = new IPEndPoint(IPAddress.Parse(IP), 65501 + portStep);

            TCPOutPackets.Add(new Packet(TCPOutPoint, NETID));
            TCPOutPackets.Add(new Packet(TCPOutPoint, 65500 + portStep));
            TCPOutPackets.Add(new Packet(TCPOutPoint, 65501 + portStep));
            StartObserversTread();
        }

        private void GetStoreData(string ip)
        {
            Vector3 StorePosition = Vector3.zero;
            string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
            string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
            SqliteConnection SQLConnection = new SqliteConnection(Params);
            SqliteCommand SQLComand = SQLConnection.CreateCommand();
            SQLConnection.Open();

            SQLComand.CommandText = "SELECT X,Y,Z,Name FROM Clients WHERE IP='" + ip +"';";
            SqliteDataReader Reader = SQLComand.ExecuteReader();

            if (Reader.HasRows)
            {
                Reader.Read();
                StorePosition = new Vector3(Reader.GetFloat("X"), Reader.GetFloat("Y"), Reader.GetFloat("Z"));
                Name = Reader.GetString("Name");
                Reader.Close();
            }

            SQLConnection.Close();

            CurrChank = new ChunkPos(((int)StorePosition.x  - (10 * (1 - (int)Mathf.Sign(StorePosition.x)))) / 20
                                     ,(int)(StorePosition.z - (10 * (1 - (int)Mathf.Sign(StorePosition.z)))) / 20) * 20;
             
            MyPosition = StorePosition;
        }

        public void Spawn()
        {
            if (MyPosition == Vector3.zero)
            {
                RaycastHit hit;
                Physics.Raycast((Vector3.up * 1000) + (Vector3.right * 0.5f) + (Vector3.forward * 0.5f), Vector3.down, out hit);
                MyPosition = hit.point + (Vector3.up * 2f);

            }
            MyObject = GameObject.Instantiate(PlayerGameObject, MyPosition, new Quaternion(0, 0, 0, 1)) as GameObject;
            MyObject.GetComponent<PlayerObserver>().PosBack = SetCurrPos;
            MyObject.GetComponent<PlayerObserver>().TriggerBack = StopСalculation;

            StartCalcTread();
        }
        
        public void SetCurrPos(Transform TForm)
        {
            MyPosition = TForm.position;
            if(IsSpawnedOnClient)
                UDPOutPackets.Add(new Packet(UDPOutPoint, TForm, NETID));
        }
        
        public Vector3 GetCurrPos()
        {
            return MyPosition;
        }

        public void SetCurrChank(ChunkPos newCurrChank)
        {

            ChunkPos Delta = newCurrChank - CurrChank;
            CurrChank = newCurrChank;
            // Запускаем скан чанков для загрузки
            ChunkToFactoryAdder ChankAdder = new ChunkToFactoryAdder(CurrChank);
            Thread FactoryAdderThread = new Thread(new ThreadStart(ChankAdder.AdderLoop));
            FactoryAdderThread.IsBackground = true;
            FactoryAdderThread.Start();

        }

        public Vector3 GetCurrObjectPoint()
        {
            return MyPosition;
        }

        private void StartObserversTread()
        {
            TCPObserverTread = new Thread( new TCPObserver(TCPInPoint, ReturnTCPPacket).ReseiveLoop);
            TCPObserverTread.IsBackground = true;
            TCPObserverTread.Start();

            UDPObserverTread = new Thread(new UDPObserver(UDPInPoint, ReturnUDPPacket).ReseiveLoop);
            UDPObserverTread.IsBackground = true;
            UDPObserverTread.Start();
        }

        public void ReturnTCPPacket(Packet InPacket)
        {
            TCPOutPackets.Add(InPacket);
            Debug.Log("Принял TCP от клиента пакет (" + InPacket.Point + ")");
        }

        public void ReturnUDPPacket(Packet InPacket)
        {
            UDPOutPackets.Add(InPacket);
            Debug.Log("Принял TCP от клиента пакет (" + InPacket.Point + ")");
        }

        private void StartCalcTread()
        {
            CurrChankCalc ChankMng = new CurrChankCalc(MyObject.name, SetCurrChank, GetCurrPos);
            CalcCurrChankThread = new Thread(new ThreadStart(ChankMng.CalcLoop));
            CalcCurrChankThread.IsBackground = true;
            CalcCurrChankThread.Start();
        }

        public void StopСalculation()
        {
            CalcCurrChankThread.Abort();
            CalcCurrChankThread.Join();
        }
    }

    public class CurrChankCalc
    {
        private bool Run = true;
        public ChunkPos CurrChankPoint;
        private Vector3 ObjectPoint;

        public delegate void OutCallback(ChunkPos point);
        public OutCallback ReturnChunkPoint; //SetCurrChunk

        public delegate Vector3 InCallback();
        public InCallback GetPos; //GetCurrPos

        public string Name;

        public CurrChankCalc(string name, OutCallback returnPoint, InCallback getPoint)
        {
            Name = name;
            ReturnChunkPoint = returnPoint;
            GetPos = getPoint;
        }

        public void CalcLoop()
        {
            Debug.Log("Вычечление позиции для ("+ Name + ") Запущено");
            while (Run)
            {
                try
                {
                    ObjectPoint = GetPos();
                    if (ObjectPoint != Vector3.zero)
                    {
                        Vector3Int point = Vector3Int.FloorToInt(ObjectPoint);
                        ChunkPos NewCurrChank = new ChunkPos((point.x - (10 * (1 - (int)Mathf.Sign(point.x)))) / 20
                                                            ,(point.z - (10 * (1 - (int)Mathf.Sign(point.z)))) / 20) * 20;

                        if (CurrChankPoint != NewCurrChank)
                        {
                            CurrChankPoint = NewCurrChank;
                            Debug.Log("Новый чанк существа (" + Name + ") (" + CurrChankPoint + ")");
                            ReturnChunkPoint(CurrChankPoint);
                        }
                    }
                    Thread.Sleep(200);

                }
                catch
                {
                    Run = false;

                }
            }
            Debug.Log("Вычечление позиции для (" + Name + ") Остановленно");

        }
    }
}
