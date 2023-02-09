using BaseObjects;
using MyNET;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.UIElements;
using static MyNET.UDPSender;

public class PlayerChankLoader : MonoBehaviour
{
    Vector3Int CurrChank;
    public delegate void InCallback(Vector3Int currChank);
    public InCallback ChankBack;
    List<Vector3Int> RenderList = new List<Vector3Int>();

    private void Start()
    {
        UpdateCurrChank();
        RenderList.Add(CurrChank + (Vector3Int.forward * 20));
        RenderList.Add(CurrChank + (Vector3Int.forward * 20) + (Vector3Int.right * 20));
        RenderList.Add(CurrChank + (Vector3Int.forward * 20) + (Vector3Int.left * 20));

        RenderList.Add(CurrChank + (Vector3Int.back * 20));
        RenderList.Add(CurrChank + (Vector3Int.back * 20) + (Vector3Int.right * 20));
        RenderList.Add(CurrChank + (Vector3Int.back * 20) + (Vector3Int.left * 20));

        RenderList.Add(CurrChank + (Vector3Int.right * 20));
        RenderList.Add(CurrChank + (Vector3Int.left * 20));

        foreach(Vector3Int point in RenderList)
        {
            RenderNearChanks(point);
        }
    }

    void FixedUpdate()
    {
        if (ChankBack != null)
        {
            UpdateCurrChank();
        }

        List<Vector3Int> tempRenderList = new List<Vector3Int>();
        tempRenderList.AddRange(RenderList);
        foreach (Vector3Int point in tempRenderList)
        {
            if (Chank.Render(point))
                 RenderList.Remove(point);
        }
    }

    private void UpdateCurrChank()
    {
        Vector3Int NewCurrChank = new Vector3Int(((int)gameObject.transform.position.x -(10 * (1 - (int)Mathf.Sign(gameObject.transform.position.x)))) / 20
                                                ,0  
                                                ,((int)gameObject.transform.position.z - (10 * (1 - (int)Mathf.Sign(gameObject.transform.position.z)))) / 20) * 20;

        if(CurrChank != NewCurrChank)
        {
            ChankBack(NewCurrChank);
            RenderNearChanks(NewCurrChank);
            CurrChank = NewCurrChank;
        }
    }

    private void RenderNearChanks(Vector3Int newCurrChank)
    {
        RenderList.Clear();

        Vector3Int Delta = newCurrChank - CurrChank;
        RenderList.Add(newCurrChank + Delta);
        StartPreRender(newCurrChank + Delta);
        if (Delta.x == 0)
        {
            RenderList.Add(newCurrChank + (Vector3Int.right * 20));
            RenderList.Add(newCurrChank + (Vector3Int.left * 20));
            StartPreRender(newCurrChank + Delta + (Vector3Int.right * 20));
            StartPreRender(newCurrChank + Delta + (Vector3Int.left * 20));
        }
        else
        {
            RenderList.Add(newCurrChank + (Vector3Int.forward * 20));
            RenderList.Add(newCurrChank + (Vector3Int.left * 20));
            StartPreRender(newCurrChank + Delta + (Vector3Int.forward * 20));
            StartPreRender(newCurrChank + Delta + (Vector3Int.back * 20));
        }

    }

    private void StartPreRender(Vector3Int chankPoint)
    {
        ChankPreRender Render = new ChankPreRender(RenderList);
        Thread RenderThread = new Thread(new ThreadStart(Render.RenderLoop));
        RenderThread.IsBackground = true;
        RenderThread.Start();
    }
}
