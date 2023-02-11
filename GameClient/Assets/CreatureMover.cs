using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMover : MonoBehaviour
{
    public delegate void InCallback(out Vector3 posotion, out Quaternion rotation);
    public InCallback PosGet;

    public delegate void TriggerCallback();
    public TriggerCallback TriggerBack;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 NewPos;
        Quaternion NewRot;
             
        
        PosGet(out NewPos, out NewRot);
        transform.position = NewPos;
        transform.rotation = NewRot;
    }
}
