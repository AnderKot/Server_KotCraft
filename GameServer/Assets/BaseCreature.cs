using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using BaseObjects;
using System.Net;
using UnityEditor.PackageManager;
using UnityEngine.Rendering;
using System.Threading;
using MyNET;
using UnityEngine.Rendering.RendererUtils;

namespace BaseCreature 
{
    public class Player
    {
        string IP;
        string Name;
        EndPoint MyEndPoint;
        public Vector3 MyPosition;
        GameObject MyObject;
        //ChankLoader ChankLoader;
        Thread CalcCurrChankThread;
        Vector3Int CurrChank;



        private static GameObject PlayerGameObject = Resources.Load<GameObject>("Player");


        public Player(string ip, int port)
        {
            IP = ip;
            MyEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
            MyPosition = GetStorePosition(ip);
            Spawn();
            StartCalcTread();
        }

        static Vector3 GetStorePosition(string ip)
        {
            Vector3 StorePosition = Vector3.zero;
            string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
            string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
            SqliteConnection SQLConnection = new SqliteConnection(Params);
            SqliteCommand SQLComand = SQLConnection.CreateCommand();
            SQLConnection.Open();

            SQLComand.CommandText = "SELECT X,Y,Z FROM Clients WHERE IP='"+ ip +"';";
            SqliteDataReader Reader = SQLComand.ExecuteReader();

            if (Reader.HasRows)
            {
                Reader.Read();
                StorePosition = new Vector3(Reader.GetFloat("X"), Reader.GetFloat("Y"), Reader.GetFloat("Z"));
                Reader.Close();
            }

            SQLConnection.Close();
            return StorePosition;
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
        }
        
        public void SetCurrPos(Transform TForm)
        {
            MyPosition = TForm.position;
            MyNETServer.OutPackets.Add(new Packet(MyEndPoint, TForm));
        }

        public Vector3 GetCurrPos()
        {
            return MyPosition;
        }

        public void SetCurrChank(Vector3Int newCurrChank)
        {

            Vector3Int Delta = newCurrChank - CurrChank;
            CurrChank = newCurrChank;

            ChunkSpawner.AddToNeadSpawnList(newCurrChank + Delta);
            if (Delta.x == 0)
            {
                ChunkSpawner.AddToNeadSpawnList(newCurrChank + Delta + (Vector3Int.right * 20));
                ChunkSpawner.AddToNeadSpawnList(newCurrChank + Delta + (Vector3Int.left * 20));
            }
            else
            {
                ChunkSpawner.AddToNeadSpawnList(newCurrChank + Delta + (Vector3Int.forward * 20));
                ChunkSpawner.AddToNeadSpawnList(newCurrChank + Delta + (Vector3Int.back * 20));
            }

        }

        public Vector3 GetCurrObjectPoint()
        {
            return MyPosition;
        }

        private void StartCalcTread()
        {
            CurrChankMng ChankMng = new CurrChankMng(MyObject.name, SetCurrChank, GetCurrPos);
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

    public class CurrChankMng
    {
        private bool Run = true;
        public Vector3Int CurrChankPoint;
        private Vector3 ObjectPoint;

        public delegate void OutCallback(Vector3Int point);
        public OutCallback ReturnChunkPoint; //SetCurrChunk

        public delegate Vector3 InCallback();
        public InCallback GetPos; //GetCurrPos

        public string Name;

        public CurrChankMng(string name, OutCallback returnPoint, InCallback getPoint)
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
                        Vector3Int NewCurrChank = new Vector3Int((point.x - (10 * (1 - (int)Mathf.Sign(point.x)))) / 20
                                                            , 0
                                                            , (point.z - (10 * (1 - (int)Mathf.Sign(point.z)))) / 20) * 20;

                        if (CurrChankPoint != NewCurrChank)
                        {
                            CurrChankPoint = NewCurrChank;
                            Debug.Log("Новый чанк существа (" + Name + ") (" + CurrChankPoint + ")");
                            ReturnChunkPoint(CurrChankPoint);
                        }
                    }
                    Thread.Sleep(100);

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
