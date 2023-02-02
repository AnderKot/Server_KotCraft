using BaseObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterMng : MonoBehaviour
{
    public bool IsRender;
    public bool IsGenerate;
    public bool IsRenderOne;
    public bool IsGenerateOne;

    public int X;
    public int Z;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 45;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsRender)
        {
            IsRender = false;
            foreach (KeyValuePair<Vector3, Chank> chank in Chank.Chanks)
            {
                chank.Value.Render();
            }
        }

        if (IsGenerate)
        {
            IsGenerate = false;
            for (int x = 0; x < 20; x++)
            {
                for (int y = -1; y < 3; y++)
                {
                    for (int z = 0; z < 20; z++)
                        Chank.AddChank(new Vector3(x * 20, y * 20, z * 20));
                }
            }
        }

        if (IsRenderOne)
        {
            IsRenderOne = false;
            Chank.Chanks[new Vector3(X * 20, 0, Z * 20)].Render();

        }

        if (IsGenerateOne)
        {
            IsGenerateOne = false;

            Chank.AddChank(new Vector3(X * 20, 0, Z * 20));

        }
    }
}
