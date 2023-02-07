using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseObjects;


public class ChankMng : MonoBehaviour
{
    private Chank MyChank;

    void Start()
    {
        

        //MyChank = new Chank(this, transform.position, );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderSelf()
    {
        MyChank.Render();
        //GetComponent<MeshFilter>().mesh = MyChank.MyMesh;
    }
}
