using BaseObjects;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using ObjectsData;

public class GameMasterMng : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Block.Load();        
        Application.targetFrameRate = 45;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    private void OnDestroy()
    {
        Block.Save();
    }

}
