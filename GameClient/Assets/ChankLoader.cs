using BaseObjects;
using MyNET;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.UIElements;

public class ChankLoader : MonoBehaviour
{
    Vector3Int CurrChank;
    public delegate void InCallback(Vector3Int currChank);
    public InCallback ChankBack;
    
    public static List<Vector3Int> RenderList = new List<Vector3Int>();

    void FixedUpdate()
    {
        if (RenderList.Count > 0)
        {
            Vector3Int CuuPoint = RenderList[0];
            if (Chank.Render(CuuPoint))
                RenderList.Remove(CuuPoint);
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


        



        


    }


}
