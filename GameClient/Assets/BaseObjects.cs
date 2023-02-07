using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;

using System.IO;
using UnityEditor;
using static UnityEditor.Progress;
using ObjectsData;
using System.Drawing;
using UnityEngine.Networking.Types;
using UnityEngine.XR;
using System.Reflection;
using Unity.VisualScripting;

namespace BaseObjects
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
        
        static public void Load()
        {
            string JsonName = @"\ObjectList.Json";
            string JsonStorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData";
            string FullFilePath = JsonStorePath + JsonName;
            if (File.Exists(FullFilePath))
            {
                string Json = File.ReadAllText(FullFilePath);
                BlocksDataList DataList = JsonUtility.FromJson<BlocksDataList>(Json);
                foreach (BlockData data in DataList.Blocks)
                {
                    Blocks.Add(data.ID, new Block(data.Name, data.Transparency));
                }
                
            }

        }

        static public void Save()
        {
            BlocksDataList DataList = new BlocksDataList(Blocks);
            string Json = JsonUtility.ToJson(DataList);
            string JsonName = @"\ObjectList.Json";
            string JsonStorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData";
            if (!Directory.Exists(JsonStorePath))
            {
                Directory.CreateDirectory(JsonStorePath);
            }

            string FullFilePath = JsonStorePath + JsonName;

            if (File.Exists(FullFilePath))
            {
                File.Delete(FullFilePath);
            }

            File.WriteAllText(FullFilePath, Json);
        }

        public static bool IsTransparency(int id)
        {
            if (!Blocks.ContainsKey(1))
                Block.Blocks.Add(3, new Block("Crass", false));

            if (!Blocks.ContainsKey(2))
                Block.Blocks.Add(2, new Block("Stone", false));

            if (!Blocks.ContainsKey(1))
                Block.Blocks.Add(1, new Block("Dirt", false));

            if (!Blocks.ContainsKey(0))
                Block.Blocks.Add(0, new Block("Void", true));

            if (Blocks.ContainsKey(id))
                return Blocks[id].Transparency;
            else
                return true;
        }
    }



    public class Chank
    {
        public Dictionary<Vector3Int, int> BlocksID = new Dictionary<Vector3Int, int>();
        //public int[,,] BloksID = new int[20, 20, 20];
        public Vector3 ChankPoint;
        public GameObject MyObject;
        public Mesh MyMesh = new Mesh();
        public List<Vector2> MyUVasID = new List<Vector2>();
        public List<Vector2> MyUV = new List<Vector2>();
        public List<MyMeshInfo> StartMeshPointers = new List<MyMeshInfo>();

        //static Vector3Int[,] Verticles = new Vector3Int[2,2];
        private List<Vector3> Verticles = new List<Vector3>();

        private List<int> Triangles = new List<int>();

        // --����������� ����--
        private static GameObject ChankGameObject = Resources.Load<GameObject>("ChankObject");
        public static Dictionary<Vector3, Chank> Chanks = new Dictionary<Vector3, Chank>();

        // --������������--
        public Chank()
        {
            //ChankMash.SetIndexBufferData
        }

        public Chank(Vector3 chankPoint)
        {
            ChankPoint = chankPoint;
            BlocksID = TerrainGenerator.Run(chankPoint);

        }
        
        public Chank(Vector3 chankPoint, Dictionary<Vector3Int, int> bloksID)
        {
            ChankPoint = chankPoint;
            BlocksID = bloksID;

        }

        public Chank(Vector3 chankPoint, List<BlocksIDData> bloksIDData)
        {
            ChankPoint = chankPoint;
            foreach (BlocksIDData idData in bloksIDData)
            {
                BlocksID.Add(new Vector3Int(idData.Point_x, idData.Point_y, idData.Point_z), idData.ID);
            }

        }

        public Chank(GameObject myObject, Vector3 chankPoint, Dictionary<Vector3Int, int> bloksID)
        {
            MyObject = myObject;
            ChankPoint = chankPoint;
            BlocksID = bloksID;
            Chanks.Add(ChankPoint, this);
        }
        

        // --����������� ������--
        public static void AddChank(Vector3 newChankPoint)
        {
            if(!Chanks.ContainsKey(newChankPoint))
            {
                Chanks.Add(newChankPoint, new Chank(newChankPoint));
            }
        }

        public static void AddChank(Vector3 newChankPoint, Dictionary<Vector3Int, int> bloksID)
        {
            if (!Chanks.ContainsKey(newChankPoint))
            {
                Chanks.Add(newChankPoint, new Chank(newChankPoint, bloksID));
            }
        }

        public static void AddChank(Vector3 newChankPoint, List<BlocksIDData> bloksIDData)
        {
            if (!Chanks.ContainsKey(newChankPoint))
            {
                Chanks.Add(newChankPoint, new Chank(newChankPoint, bloksIDData));
            }
        }
        /*
        public static void Save()
        {


            ChankDataList DataList = new ChankDataList(Chanks);

            string Json = JsonUtility.ToJson(DataList);
            string JsonName = @"\ChankList.Json";
            


        }


        public static void Load()
        {
            string JsonName = @"\ChankList.Json";
            string JsonStorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData";
            string FullFilePath = JsonStorePath + JsonName;
            if (File.Exists(FullFilePath))
            {
                string Json = File.ReadAllText(FullFilePath);
                ChankDataList DataList = JsonUtility.FromJson<ChankDataList>(Json);
                foreach (ChankData chankData in DataList.Chanks)
                {
                    Chanks.Add(chankData.Point, new Chank(chankData.Point, chankData.BlocksID));
                }

            }
        }
        */
        //--����������--
        public void Render()
        {
            GenerateMesh();

            UpdateObjectMash();
        }

        private void UpdateObjectMash()
        {
            MyMesh.Clear();
            MyMesh.vertices = Verticles.ToArray();
            MyMesh.triangles = Triangles.ToArray();
            //MyMesh.uv
            MyMesh.uv = MyUVasID.ToArray();
            MyMesh.uv2 = MyUV.ToArray();

            MyMesh.RecalculateBounds();
            MyMesh.RecalculateNormals();

            if (!MyObject)
            {
                MyObject = GameObject.Instantiate(ChankGameObject) as GameObject;
                MyObject.transform.position = this.ChankPoint;
            }

            if (MyObject)
            {
                MyObject.GetComponent<MeshFilter>().sharedMesh = MyMesh;
                MyObject.GetComponent<MeshCollider>().sharedMesh = MyMesh;
            }
        }

        public void Show ()
        {
            
            MyObject.SetActive(true);
        }

        public void Hide()
        {
            MyObject.SetActive(false);
        }



        //--���������--
        private int GetLocalBlockID(Vector3Int BlockPoint)
        {
            Vector3 OtherChankPoint = this.ChankPoint;
            Vector3Int OtheBlockPoint = BlockPoint;
            int BlockID = 0;

            if (BlockPoint.x < 0)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.left * 20);
                OtheBlockPoint += Vector3Int.right * 20; 
            }
            if (19 < BlockPoint.x)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.right * 20);
                OtheBlockPoint += Vector3Int.left * 20;
            }
            
            if (BlockPoint.y < 0)
            {
                //OtherChankPoint = this.ChankPoint + (Vector3Int.down * 20);
                //OtheBlockPoint += Vector3Int.up * 20;
                return BlockID;
            }
            /*
            if (19 < BlockPoint.y)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.up * 20);
                OtheBlockPoint += Vector3Int.down * 20;
            }
            */
            if (BlockPoint.z < 0)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.back * 20);
                OtheBlockPoint += Vector3Int.forward * 20;
            }
            if (19 < BlockPoint.z)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.forward * 20);
                OtheBlockPoint += Vector3Int.back * 20;
            }

            if (Chanks.ContainsKey(OtherChankPoint))
            {
                 Chanks[OtherChankPoint].BlocksID.TryGetValue(OtheBlockPoint, out BlockID);
            }
            return BlockID;
        }

        public void SetBlock(Vector3Int point, int id)
        {
            if(!BlocksID.ContainsKey(point))
            {
                BlocksID.Add(point, id);
                AddBlockMash(point, id);

                if (!Block.IsTransparency(id))
                {
                    CutSideBlockMash(point + Vector3Int.up, 2);
                    CutSideBlockMash(point + Vector3Int.down, 1);
                    CutSideBlockMash(point + Vector3Int.forward, 4);
                    CutSideBlockMash(point + Vector3Int.back, 3);
                    CutSideBlockMash(point + Vector3Int.left, 6);
                    CutSideBlockMash(point + Vector3Int.right, 5);
                }

                UpdateObjectMash();
            }

        }

        public void DeleteBlock(Vector3Int point, int id)
        {
            if (BlocksID.ContainsKey(point))
            {
                //AddBlockMash(point, id);
                List<int> NearBlocksSides = CutAllBlockMash(point);
                BlocksID.Remove(point);

                int OldVerticlesCount = Verticles.Count;
                int OldTrianglesCount = Triangles.Count;
                int ID;
                foreach (int sides in NearBlocksSides)
                {
                    switch (sides)
                    {
                        case 1:
                            ID = GetLocalBlockID(point + Vector3Int.up);
                            if (ID != 0)
                            {
                                AddDownSideMesh(point + Vector3Int.up, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 2:
                            ID = GetLocalBlockID(point + Vector3Int.down);
                            if (ID != 0)
                            {
                                AddUpSideMesh(point + Vector3Int.down, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 3:
                            ID = GetLocalBlockID(point + Vector3Int.forward);
                            if (ID != 0)
                            {
                                AddBackSideMesh(point + Vector3Int.forward, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 4:
                            ID = GetLocalBlockID(point + Vector3Int.back);
                            if (ID != 0)
                            {
                                AddForwardSideMesh(point + Vector3Int.back, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 5:
                            ID = GetLocalBlockID(point + Vector3Int.left);
                            if (ID != 0)
                            {
                                AddRightSideMesh(point + Vector3Int.left, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 6:
                            ID = GetLocalBlockID(point + Vector3Int.right);
                            if (ID != 0)
                            {
                                AddLeftSideMesh(point + Vector3Int.right, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                    }
                }
                UpdateObjectMash();
            }
        }

        private List<int> CutAllBlockMash(Vector3Int point)
        {
            int[] Sides = { 1, 2, 3, 4, 5, 6 }; 
            List<int> NearBlocksSides = new List<int>(Sides);
            if (BlocksID.ContainsKey(point))
            {
                bool Transparency = Block.IsTransparency(GetLocalBlockID(point));
                List<MyMeshInfo> Pointers = StartMeshPointers.FindAll(pointer => (pointer.BlockPoint == point));
                foreach (MyMeshInfo Pointer in Pointers)
                {
                    if (!Transparency)
                        NearBlocksSides.Remove(Pointer.SideID);
                    CutSideBlockMash(Pointer.BlockPoint, Pointer.SideID);
                }
                
                        /*
                        List<MyMeshInfo> Pointers = StartMeshPointers.FindAll(pointer => (pointer.BlockPoint == point));
                        int DeletedIndex = StartMeshPointers.IndexOf(Pointers[0]);
                        //int CountDeletedID = 0;
                        int DST = 0;
                        int DSV = 0;
                        bool Transparency = Block.IsTransparency(GetLocalBlockID(point));
                        foreach (MyMeshInfo Pointer in Pointers)
                        {
                            if (!Transparency)
                                NearBlocksSides.Remove(Pointer.SideID);

                            Verticles.RemoveRange(Pointer.StartVerticlIndex - DSV, 4);
                            MyUV.RemoveRange(Pointer.StartVerticlIndex - DSV, 4);
                            MyUVasID.RemoveRange(Pointer.StartVerticlIndex - DSV, 4);
                            Triangles.RemoveRange(Pointer.StartTrianglesIndex - DST, 6);
                            List<int> TempTriangles = Triangles.GetRange(Pointer.StartTrianglesIndex - DST, Triangles.Count - (Pointer.StartTrianglesIndex - DST));
                            Triangles.RemoveRange(Pointer.StartTrianglesIndex - DST, Triangles.Count - (Pointer.StartTrianglesIndex - DST));
                            foreach (int triangles in TempTriangles)
                            {
                                Triangles.Add(triangles - 4);
                            }

                            DeletedIndex = StartMeshPointers.FindIndex(pointer => (pointer.BlockPoint == Pointer.BlockPoint) & (pointer.SideID == Pointer.SideID));
                            StartMeshPointers.RemoveAt(DeletedIndex);
                            List<MyMeshInfo> TempPointers = StartMeshPointers.GetRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
                            StartMeshPointers.RemoveRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
                            foreach (MyMeshInfo pointers in TempPointers)
                            {
                                StartMeshPointers.Add(new MyMeshInfo(pointers.BlockPoint, pointers.StartVerticlIndex - DSV, pointers.StartTrianglesIndex - DST, pointers.SideID));
                            }

                            //CountDeletedID++;
                            DST += 6;
                            DSV += 4;
                        }
                        */
                        /*
                        StartMeshPointers.RemoveRange(DeletedIndex, CountDeletedID);
                        List<MyMeshInfo> TempPointers = StartMeshPointers.GetRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
                        StartMeshPointers.RemoveRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
                        foreach (MyMeshInfo pointers in TempPointers)
                        {
                            StartMeshPointers.Add(new MyMeshInfo(pointers.BlockPoint, pointers.StartVerticlIndex - (4 * CountDeletedID), pointers.StartTrianglesIndex - (6 * CountDeletedID), pointers.SideID));
                        }
                        */
            }
            return NearBlocksSides;
        }

        private void CutSideBlockMash(Vector3Int point, int sideID)
        {
            if (BlocksID.ContainsKey(point))
            {
                MyMeshInfo CurrInfo = StartMeshPointers.Find(pointer => (pointer.BlockPoint == point) & (pointer.SideID == sideID));
                if(CurrInfo.SideID != 0)
                {
                    Verticles.RemoveRange(CurrInfo.StartVerticlIndex, 4);
                    MyUV.RemoveRange(CurrInfo.StartVerticlIndex, 4);
                    MyUVasID.RemoveRange(CurrInfo.StartVerticlIndex, 4);
                    Triangles.RemoveRange(CurrInfo.StartTrianglesIndex, 6);
                    List<int> TempTriangles = Triangles.GetRange(CurrInfo.StartTrianglesIndex, Triangles.Count - CurrInfo.StartTrianglesIndex);
                    Triangles.RemoveRange(CurrInfo.StartTrianglesIndex, Triangles.Count - CurrInfo.StartTrianglesIndex);
                    foreach (int triangles in TempTriangles)
                    {
                        Triangles.Add(triangles - 4);
                    }
                
                    int DeletedIndex = StartMeshPointers.IndexOf(CurrInfo);
                    StartMeshPointers.RemoveAt(DeletedIndex);
                    List<MyMeshInfo> TempPointers = StartMeshPointers.GetRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
                    StartMeshPointers.RemoveRange(DeletedIndex, StartMeshPointers.Count - DeletedIndex);
                    foreach (MyMeshInfo pointers in TempPointers)
                    {
                        StartMeshPointers.Add(new MyMeshInfo(pointers.BlockPoint, pointers.StartVerticlIndex - 4, pointers.StartTrianglesIndex - 6, pointers.SideID));
                    }
                }
            }
        }
        

        private void GenerateMesh()
        {

            Verticles.Clear();
            Triangles.Clear();
            MyUV.Clear();
            MyUVasID.Clear();
            StartMeshPointers.Clear();

            foreach (KeyValuePair<Vector3Int, int> currBlock in this.BlocksID) 
            {
                if (currBlock.Value == 0) return;

                AddBlockMash(currBlock.Key, currBlock.Value);
            }
        }

        private void AddBlockMash(Vector3Int point, int id)
        {
            int OldVerticlesCount = Verticles.Count;
            int OldTrianglesCount = Triangles.Count;

            AddUpSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

            AddDownSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

            AddForwardSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

            AddBackSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

            AddLeftSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

            AddRightSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);
        }

        private void AddRightSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(GetLocalBlockID(point + Vector3Int.right))) //6
            {
                Verticles.Add(point + new Vector3(1, 0, 0)); //0
                Verticles.Add(point + new Vector3(1, 0, 1)); //1
                Verticles.Add(point + new Vector3(1, 1, 0)); //2
                Verticles.Add(point + new Vector3(1, 1, 1)); //3
                MyUV.Add(new Vector2(0, 0));
                MyUV.Add(new Vector2(1, 0));
                MyUV.Add(new Vector2(0, 1));
                MyUV.Add(new Vector2(1, 1));

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                StartMeshPointers.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 6));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddLeftSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(GetLocalBlockID(point + Vector3Int.left))) //5
            {
                Verticles.Add(point + new Vector3(0, 0, 0)); //0
                Verticles.Add(point + new Vector3(0, 1, 0)); //2
                Verticles.Add(point + new Vector3(0, 0, 1)); //1
                Verticles.Add(point + new Vector3(0, 1, 1)); //3
                MyUV.Add(new Vector2(0, 0));
                MyUV.Add(new Vector2(0, 1));
                MyUV.Add(new Vector2(1, 0));
                MyUV.Add(new Vector2(1, 1));

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                StartMeshPointers.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 5));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddBackSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(GetLocalBlockID(point + Vector3Int.back))) //4
            {
                Verticles.Add(point + new Vector3(0, 0, 0)); //0
                Verticles.Add(point + new Vector3(1, 0, 0)); //1
                Verticles.Add(point + new Vector3(0, 1, 0)); //2
                Verticles.Add(point + new Vector3(1, 1, 0)); //3
                MyUV.Add(new Vector2(0, 0));
                MyUV.Add(new Vector2(1, 0));
                MyUV.Add(new Vector2(0, 1));
                MyUV.Add(new Vector2(1, 1));

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                StartMeshPointers.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 4));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddForwardSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(GetLocalBlockID(point + Vector3Int.forward))) //3
            {
                Verticles.Add(point + new Vector3(0, 0, 1)); //0
                Verticles.Add(point + new Vector3(0, 1, 1)); //2
                Verticles.Add(point + new Vector3(1, 0, 1)); //1
                Verticles.Add(point + new Vector3(1, 1, 1)); //3
                MyUV.Add(new Vector2(0, 0));
                MyUV.Add(new Vector2(0, 1));
                MyUV.Add(new Vector2(1, 0));
                MyUV.Add(new Vector2(1, 1));

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                StartMeshPointers.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 3));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddDownSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(GetLocalBlockID(point + Vector3Int.down))) //2
            {
                Verticles.Add(point + new Vector3(0, 0, 0)); //0
                Verticles.Add(point + new Vector3(0, 0, 1)); //2
                Verticles.Add(point + new Vector3(1, 0, 0)); //1
                Verticles.Add(point + new Vector3(1, 0, 1)); //3
                MyUV.Add(new Vector2(0, 0));
                MyUV.Add(new Vector2(1, 0));
                MyUV.Add(new Vector2(0, 1));
                MyUV.Add(new Vector2(1, 1));

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                StartMeshPointers.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 2));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddUpSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(GetLocalBlockID(point + Vector3Int.up)))  //1
            {
                Verticles.Add(point + new Vector3(0, 1, 0)); //0
                Verticles.Add(point + new Vector3(1, 1, 0)); //1
                Verticles.Add(point + new Vector3(0, 1, 1)); //2
                Verticles.Add(point + new Vector3(1, 1, 1)); //3
                MyUV.Add(new Vector2(0, 0));
                MyUV.Add(new Vector2(1, 0));
                MyUV.Add(new Vector2(0, 1));
                MyUV.Add(new Vector2(1, 1));

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                StartMeshPointers.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 1));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddDataInUV(int blockID)
        {
            MyUVasID.Add(new Vector2(blockID, 0));
            MyUVasID.Add(new Vector2(blockID, 0));
            MyUVasID.Add(new Vector2(blockID, 0));
            MyUVasID.Add(new Vector2(blockID, 0));


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

    public struct MyMeshInfo
    {
        public Vector3Int BlockPoint;
        public int StartVerticlIndex;
        public int StartTrianglesIndex;
        public int SideID;

        public MyMeshInfo(Vector3Int point, int startVerticlesCount, int startTrianglesCount, int sideID) : this()
        {
            this.BlockPoint = point;
            this.StartVerticlIndex = startVerticlesCount;
            this.StartTrianglesIndex = startTrianglesCount;
            this.SideID = sideID;
        }
    }


}
