using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseObjects;


public class ChankMng : MonoBehaviour
{
    private void OnDestroy()
    {
        Chank.Chanks[Vector3Int.FloorToInt(transform.position)].Save();
    }

}
