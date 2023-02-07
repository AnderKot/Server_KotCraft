using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseObjects;


public class ChankMng : MonoBehaviour
{
    private void OnDestroy()
    {
        Chank.Chanks[transform.position].Save();
    }

}
