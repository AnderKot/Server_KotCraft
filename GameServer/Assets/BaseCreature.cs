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

namespace BaseCreature 
{
    public class Player
    {
        string IP;
        string Name;
        public Vector3 MyPosition;
        GameObject MyObject;
        Vector3Int CurrChank;
        Vector3Int CurrLocals;

        private static GameObject PlayerGameObject = Resources.Load<GameObject>("Player");


        public Player(string ip)
        {
            IP = ip;
            MyPosition = GetStorePosition(ip);
            CalcCurrChank();
            Spawn();
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
            if(!Chank.Chanks.ContainsKey(CurrChank))
            {
                Chank.AddChank(CurrChank);
                Chank.PreRender(CurrChank);
            }

            if (Chank.Chanks[CurrChank].MyObject == null)
                Chank.Chanks[CurrChank].Render();
            else
                Chank.Chanks[CurrChank].Show();

            if (MyPosition == Vector3.zero)
            {
                RaycastHit hit;
                Physics.Raycast((Vector3.up * 1000) + (Vector3.right * 0.5f) + (Vector3.forward * 0.5f), Vector3.down, out hit);
                MyPosition = hit.point + (Vector3.up * 2f);

            }
            MyObject = GameObject.Instantiate(PlayerGameObject, MyPosition, new Quaternion(0, 0, 0, 1)) as GameObject;
            MyObject.GetComponent<PlayerChankLoader>().ChankBack = SetCurrChank;
        }

        private void CalcCurrChank()
        {
            CurrChank = new Vector3Int((int)MyPosition.x / 20
                                      ,0
                                      ,(int)MyPosition.z / 20) * 20;

            CurrLocals = Vector3Int.FloorToInt(MyPosition) - CurrChank;
        }

        public void SetCurrChank(Vector3Int currChank)
        {
            CurrChank = currChank;
        }
    }
}
