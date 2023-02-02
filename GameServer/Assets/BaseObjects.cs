using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;

namespace BaseObject
{
    public class Block
    {
        public string Name;
        public bool Transparency;

        static public Dictionary<int, Block> Blocks = new Dictionary<int, Block>();
        
        public Block(){}
        
        public Block(string name, bool transparency)
        {
            Name = name;
            Transparency = transparency;
        }

        static public void LoadBloks()
        {

        }
        
        static Block Load(int id)
        {
            return new Block();
        }

        public static bool IsTransparency(int id)
        {
            if (!Blocks.ContainsKey(1))
                Block.Blocks.Add(1, new Block("Default", false));
            if (!Blocks.ContainsKey(0))
                Block.Blocks.Add(0, new Block("Void", true));

            if (Blocks.ContainsKey(id))
                return Blocks[id].Transparency;
            else
                return false;
        }
    }

    [Serializable]
    public class BlockData
    {
        public int Id;
        public string Name;
        public bool Transparency;
    }




    public class Chank
    {
        public int[,,] BloksID = new int[20, 20, 20];
        public Vector3 ChankPoint;
        public GameObject MyObject;
        public Mesh MyMesh = new Mesh();

        //static Vector3Int[,] Verticles = new Vector3Int[2,2];
        private List<Vector3> Verticles = new List<Vector3>();

        private List<int> Triangles = new List<int>();

        public Chank()
        {
            //ChankMash.SetIndexBufferData
        }

        public Chank(Vector3 chankPoint)
        {
            ChankPoint = chankPoint;
            BloksID = TerrainGenerator.Run(chankPoint);
            MyObject = GameObject.Instantiate(ChankGameObject) as GameObject;
            MyObject.transform.position = chankPoint;
        }
        
        public Chank(Vector3 chankPoint, int[,,] bloksID)
        {
            ChankPoint = chankPoint;
            BloksID = bloksID;
            MyObject = GameObject.Instantiate(ChankGameObject) as GameObject;
            MyObject.transform.position = chankPoint;
        }

        public Chank(GameObject myObject, Vector3 chankPoint, int[,,] bloksID)
        {
            MyObject = myObject;
            ChankPoint = chankPoint;
            BloksID = bloksID;
            Chanks.Add(ChankPoint, this);
        }
        
        public static GameObject ChankGameObject = Resources.Load<GameObject>("ChankObject");
        public static Dictionary<Vector3, Chank> Chanks = new Dictionary<Vector3, Chank>();

        public static void AddChank(Vector3 newChankPoint)
        {
            if(!Chanks.ContainsKey(newChankPoint))
            {
                Chanks.Add(newChankPoint, new Chank(newChankPoint));
            }
        }

        public static void AddChank(Vector3 newChankPoint, int[,,] bloksID)
        {
            if (!Chanks.ContainsKey(newChankPoint))
            {
                Chanks.Add(newChankPoint, new Chank(newChankPoint, bloksID));
            }
        }
        
        public void Render()
        {
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    for (int z = 0; z < 20; z++)
                    {
                        GenerateBlock(x, y, z);
                    }
                }
            }

            MyMesh.vertices = Verticles.ToArray();
            MyMesh.triangles = Triangles.ToArray();

            MyMesh.RecalculateBounds();
            MyMesh.RecalculateNormals();

            if(MyObject)
            {
                MyObject.GetComponent<MeshFilter>().sharedMesh = MyMesh;
                MyObject.GetComponent<MeshCollider>().sharedMesh = MyMesh;
                Verticles.Clear();
                Triangles.Clear();
            }
        }

        public void Hide()
        {

        }

        private int GetBlockID(Vector3Int BlockPoint)
        {
            Vector3 OtherChankPoint = this.ChankPoint;
            Vector3Int OtheBlockPoint = BlockPoint;

            if (BlockPoint.x < 0)
            {
                OtherChankPoint = this.ChankPoint + new Vector3(-20f, 0f, 0f);
                OtheBlockPoint += new Vector3Int(20,0,0); 
            }
            if (19 < BlockPoint.x)
            {
                OtherChankPoint = this.ChankPoint + new Vector3(20f, 0f, 0f);
                OtheBlockPoint += new Vector3Int(-20, 0, 0);
            }
            if (BlockPoint.y < 0)
            {
                OtherChankPoint = this.ChankPoint + new Vector3(0f, -20f, 0f);
                OtheBlockPoint += new Vector3Int(0, 20, 0);
            }
            if (19 < BlockPoint.y)
            {
                OtherChankPoint = this.ChankPoint + new Vector3(0f, 20f, 0f);
                OtheBlockPoint += new Vector3Int(0, -20, 0);
            }
            if (BlockPoint.z < 0)
            {
                OtherChankPoint = this.ChankPoint + new Vector3(0f, 0f, -20f);
                OtheBlockPoint += new Vector3Int(0, 0, 20);
            }
            if (19 < BlockPoint.z)
            {
                OtherChankPoint = this.ChankPoint + new Vector3(0f, 0f, 20f);
                OtheBlockPoint += new Vector3Int(0, 0, -20);
            }

            if (Chanks.ContainsKey(OtherChankPoint))
            {
                return Chanks[OtherChankPoint].BloksID[OtheBlockPoint.x, OtheBlockPoint.y, OtheBlockPoint.z];
            }

            return -1;
            

        }

        private void GenerateBlock(int x, int y, int z)
        {
            if (BloksID[x, y, z] == 0) return;

            int OldCount = Verticles.Count;
            Vector3 BlockPoint = new Vector3(x, y, z);

            if (Block.IsTransparency(GetBlockID( new Vector3Int(x, y + 1, z))))
            {
                Verticles.Add(BlockPoint + new Vector3(0, 1, 0)); //0
                Verticles.Add(BlockPoint + new Vector3(1, 1, 0)); //1
                Verticles.Add(BlockPoint + new Vector3(0, 1, 1)); //2
                Verticles.Add(BlockPoint + new Vector3(1, 1, 1)); //3

                AddTriengles(OldCount);
                OldCount += 4;
            }
            
            if (Block.IsTransparency(GetBlockID(new Vector3Int(x, y - 1, z))))
            {
                Verticles.Add(BlockPoint + new Vector3(0, 0, 0)); //0
                Verticles.Add(BlockPoint + new Vector3(0, 0, 1)); //2
                Verticles.Add(BlockPoint + new Vector3(1, 0, 0)); //1
                Verticles.Add(BlockPoint + new Vector3(1, 0, 1)); //3

                AddTriengles(OldCount);
                OldCount += 4;
            }
            
            if (Block.IsTransparency(GetBlockID(new Vector3Int(x, y, z + 1))))
            {
                Verticles.Add(BlockPoint + new Vector3(0, 0, 1)); //0
                Verticles.Add(BlockPoint + new Vector3(0, 1, 1)); //2
                Verticles.Add(BlockPoint + new Vector3(1, 0, 1)); //1
                Verticles.Add(BlockPoint + new Vector3(1, 1, 1)); //3

                AddTriengles(OldCount);
                OldCount += 4;
            }
            
            if (Block.IsTransparency(GetBlockID(new Vector3Int(x, y, z - 1))))
            {
                Verticles.Add(BlockPoint + new Vector3(0, 0, 0)); //0
                Verticles.Add(BlockPoint + new Vector3(1, 0, 0)); //1
                Verticles.Add(BlockPoint + new Vector3(0, 1, 0)); //2
                Verticles.Add(BlockPoint + new Vector3(1, 1, 0)); //3

                AddTriengles(OldCount);
                OldCount += 4;
            }
            
            if (Block.IsTransparency(GetBlockID(new Vector3Int(x - 1, y, z))))
            {
                Verticles.Add(BlockPoint + new Vector3(0, 0, 0)); //0
                Verticles.Add(BlockPoint + new Vector3(0, 1, 0)); //2
                Verticles.Add(BlockPoint + new Vector3(0, 0, 1)); //1
                Verticles.Add(BlockPoint + new Vector3(0, 1, 1)); //3

                AddTriengles(OldCount);
                OldCount += 4;
            }
            
            if (Block.IsTransparency(GetBlockID(new Vector3Int(x + 1, y, z))))
            {
                Verticles.Add(BlockPoint + new Vector3(1, 0, 0)); //0
                Verticles.Add(BlockPoint + new Vector3(1, 0, 1)); //1
                Verticles.Add(BlockPoint + new Vector3(1, 1, 0)); //2
                Verticles.Add(BlockPoint + new Vector3(1, 1, 1)); //3

                AddTriengles(OldCount);
            }


        }

        private void AddTriengles(int oldCount)
        {
            Triangles.Add(oldCount + 0);
            Triangles.Add(oldCount + 2);
            Triangles.Add(oldCount + 1);

            Triangles.Add(oldCount + 3);
            Triangles.Add(oldCount + 1);
            Triangles.Add(oldCount + 2);
        }
    }

    struct Traengle
    {
        int StartVerticlIndex;
        int MiddleVerticIndex;
        int EndVerticlIndex;
    }
}

