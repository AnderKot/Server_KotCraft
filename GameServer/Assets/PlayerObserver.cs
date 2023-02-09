using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObserver : MonoBehaviour
{
    public delegate void OutCallback(Vector3 currChank);
    public OutCallback PosBack;

    public delegate void TriggerCallback();
    public TriggerCallback TriggerBack;

    // Update is called once per frame
    void FixedUpdate()
    {
        PosBack(gameObject.transform.position);
    }

    private void OnDestroy()
    {
        TriggerBack();
    }
}
