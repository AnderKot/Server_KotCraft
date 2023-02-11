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
        public Quaternion MyRotation;
        GameObject MyObject;
        //ChankLoader ChankLoader;
        Thread CalcCurrChankThread;
        Vector3Int CurrChank;



        private static GameObject PlayerGameObject = Resources.Load<GameObject>("Player");


        public Player()
        {
            Spawn();
        }

        public void Spawn()
        {
            MyObject = GameObject.Instantiate(PlayerGameObject, new Vector3(0,0,0), new Quaternion(0, 0, 0, 1)) as GameObject;
            MyObject.GetComponent<CreatureMover>().PosGet = SetCurrTForm;
        }
        
        public void SetCurrPos(Transform TForm)
        {
            MyPosition = TForm.position;
        }

        public Vector3 GetCurrPos()
        {
            return MyPosition;
        }

        public void GetCurrTForm(Transform TForm)
        {
            MyObject.transform.position = TForm.transform.position;
            MyObject.transform.rotation = TForm.transform.rotation;
        }

        public void SetCurrTForm(out Vector3 position, out Quaternion rotation)
        {
            position = MyPosition;
            rotation = MyRotation;
        }

        public void SetCurrChank(Vector3Int newCurrChank)
        {

            Vector3Int Delta = newCurrChank - CurrChank;
            CurrChank = newCurrChank;
            
            StartPreRender(newCurrChank + Delta);
            if (Delta.x == 0)
            {
                StartPreRender(newCurrChank + Delta + (Vector3Int.right * 20));
                StartPreRender(newCurrChank + Delta + (Vector3Int.left * 20));
            }
            else
            {
                StartPreRender(newCurrChank + Delta + (Vector3Int.forward * 20));
                StartPreRender(newCurrChank + Delta + (Vector3Int.back * 20));
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

        private void StartPreRender(Vector3Int chankPoint)
        {
            ChankPreRender Render = new ChankPreRender(chankPoint);
            Thread RenderThread = new Thread(new ThreadStart(Render.RenderLoop));
            RenderThread.IsBackground = true;
            RenderThread.Start();
        }
    }

    public class CurrChankMng
    {
        private bool Run = true;
        public Vector3Int CurrChankPoint;
        private Vector3 ObjectPoint;

        public delegate void OutCallback(Vector3Int point);
        public OutCallback ReturnPoint;

        public delegate Vector3 InCallback();
        public InCallback GetPoint;

        public string Name;

        public CurrChankMng(string name, OutCallback returnPoint, InCallback getPoint)
        {
            Name = name;
            ReturnPoint = returnPoint;
            GetPoint = getPoint;
        }

        public void CalcLoop()
        {
            Debug.Log("Вычечления позиции для существа ("+ Name + ") Запущено");
            while (Run)
            {
                try
                {
                    ObjectPoint = GetPoint();
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
                            ReturnPoint(CurrChankPoint);
                        }
                    }
                    Thread.Sleep(100);

                }
                catch
                {
                    Run = false;

                }
            }
            Debug.Log("Вычечления позиции для существа(" + Name + ") Остановленно");

        }
    }
}
