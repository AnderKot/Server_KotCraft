using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObserver : MonoBehaviour
{
    public delegate void OutCallback(Transform TForm);
    public OutCallback PosBack;

    public delegate void TriggerCallback();
    public TriggerCallback TriggerBack;

    // Update is called once per frame
    void FixedUpdate()
    {
        PosBack(gameObject.transform);
    }

    private void OnDestroy()
    {
        TriggerBack();
    }
}
