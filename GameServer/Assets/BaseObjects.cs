using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;

using System.IO;
using ObjectsData;
using Mono.Data.Sqlite;
using System.Data;
using System.Threading;
using System.ComponentModel;
using UnityEngine.Rendering.RendererUtils;
using MyNET;
using System.Drawing;
using Workers;

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



    public class Chunk
    {
        // --Состояния--
        public bool IsLoad = false;
        public bool IsLoaded = false;
        public bool IsModifyed = false;
        public bool IsBuilded = false;
        public bool IsBuild = false;
        public bool IsSpawn = false;
        public bool IsShow = false;

        public Dictionary<Vector3Int, int> Blocks = new Dictionary<Vector3Int, int>();
        public Vector3Int ChankPoint;
        public GameObject MyObject;


        // --Геометрия--
        public ChunkGeometry Geometry;

        // --Статические поля--
        private static GameObject GameObject = Resources.Load<GameObject>("ChankObject");
        public static Dictionary<Vector3Int, Chunk> Alphabet = new Dictionary<Vector3Int, Chunk>();

        // --Конструкторы--
        public Chunk()
        {
            //ChankMash.SetIndexBufferData
        }

        public Chunk(Vector3Int chankPoint)
        {
            ChankPoint = chankPoint;
            Blocks = RunLoad(chankPoint);
            if (Blocks == null)
            {
                Blocks = TerrainGenerator.Run(chankPoint);
                IsModifyed = true;
            }
        }

        public Chunk(Vector3Int chankPoint, Dictionary<Vector3Int, int> bloksID)
        {
            ChankPoint = chankPoint;
            Blocks = bloksID;

        }

        public Chunk(Vector3Int chankPoint, List<BlocksIDData> bloksIDData)
        {
            ChankPoint = chankPoint;
            foreach (BlocksIDData idData in bloksIDData)
            {
                Blocks.Add(new Vector3Int(idData.Point_x, idData.Point_y, idData.Point_z), idData.ID);
            }

        }

        public Chunk(GameObject myObject, Vector3Int chankPoint, Dictionary<Vector3Int, int> bloksID)
        {
            MyObject = myObject;
            ChankPoint = chankPoint;
            Blocks = bloksID;
            Alphabet.Add(ChankPoint, this);
        }

        public Chunk(Vector3Int chankPoint, int[,,] bloksIDData)
        {
            ChankPoint = chankPoint;
            for (int x = 0; x < bloksIDData.GetLength(0); x++)
            {
                for (int y = 0; y < bloksIDData.GetLength(1); y++)
                {
                    for (int z = 0; z < bloksIDData.GetLength(2); z++)
                    {
                        if (bloksIDData[x, y, z] != 0)
                            Blocks.Add(new Vector3Int(x, y, z), bloksIDData[x, y, z]);
                    }
                }
            }

        }

        // --Статические метожы--
        public static void AddChank(Vector3Int newChankPoint)
        {
            if (!Alphabet.ContainsKey(newChankPoint))
            {
                Alphabet.TryAdd(newChankPoint, new Chunk(newChankPoint));
            }
        }

        public static void AddChank(Vector3Int newChankPoint, Dictionary<Vector3Int, int> bloksID)
        {
            if (!Alphabet.ContainsKey(newChankPoint))
            {
                Alphabet.Add(newChankPoint, new Chunk(newChankPoint, bloksID));
            }
        }

        public static void AddChank(Vector3Int newChankPoint, List<BlocksIDData> bloksIDData)
        {
            if (!Alphabet.ContainsKey(newChankPoint))
            {
                Alphabet.Add(newChankPoint, new Chunk(newChankPoint, bloksIDData));
            }
        }

        public static void AddChank(Vector3Int newChankPoint, int[,,] bloksIDData)
        {
            if (!Alphabet.ContainsKey(newChankPoint))
            {
                Alphabet.Add(newChankPoint, new Chunk(newChankPoint, bloksIDData));
            }
        }

        private Dictionary<Vector3Int, int> RunLoad(Vector3 newChankPoint)
        {
            string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
            string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
            SqliteConnection SQLConnection = new SqliteConnection(Params);
            SqliteCommand SQLComand = SQLConnection.CreateCommand();
            SQLConnection.Open();
            // чанки иблоки
            SQLComand.CommandText = "SELECT ID FROM Shanks WHERE WorldID = 1 AND X=" + newChankPoint.x + " AND Y=" + newChankPoint.y + " AND Z=" + newChankPoint.z + ";";
            SqliteDataReader Reader = SQLComand.ExecuteReader();
            if (!Reader.HasRows) // - мир пустой или не найден
            {
                Reader.Close();
                return null;
            }

            Reader.Read();

            Dictionary<Vector3Int, int> LoadBloksID = new Dictionary<Vector3Int, int>();
            string CommandText = "SELECT X , Y , Z , ID FROM Blocks WHERE ShankID = " + Reader["ID"] + ";";
            Reader.Close();
            SQLComand.CommandText = CommandText;
            Reader = SQLComand.ExecuteReader();

            if (!Reader.HasRows) // - чанк пустой берем смледующий
            {
                Reader.Close();
                SQLConnection.Close();
                return null;
            }

            while (Reader.Read())
            {

                LoadBloksID.Add(new Vector3Int(Reader.GetInt32("X"), Reader.GetInt32("Y"), Reader.GetInt32("Z")), Reader.GetInt32("ID"));
            }
            Reader.Close();
            SQLConnection.Close();
            return LoadBloksID;
        }

        public void Save()
        {
            Alphabet.Remove(ChankPoint);
            if (IsModifyed)
            {
                Thread RunerThread;
                string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
                string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
                SqliteConnection SQLConnection = new SqliteConnection(Params);
                SqliteCommand SQLCommand = SQLConnection.CreateCommand();
                SQLConnection.Open();

                // - чанки и блоки
                SQLCommand.CommandText = "SELECT ID FROM Shanks WHERE WorldID='1'  AND X=" + this.ChankPoint.x + " AND Y=" + this.ChankPoint.y + " AND Z=" + this.ChankPoint.z + ";";
                SqliteDataReader Reader = SQLCommand.ExecuteReader();
                int id;

                if (Reader.HasRows)
                {
                    Reader.Read();
                    id = Reader.GetInt32("ID");
                    Reader.Close();
                }
                else
                {
                    Reader.Close();
                    SQLCommand.CommandText = "SELECT ID FROM Shanks WHERE WorldID='1' ORDER BY id DESC LIMIT 1;";
                    Reader = SQLCommand.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        Reader.Read();
                        id = Reader.GetInt32("ID") + 1;
                        Reader.Close();
                    }
                    else
                    {
                        Reader.Close();
                        id = 0;
                    }
                    SQLCommand.CommandText = "INSERT INTO Shanks (WorldID , X , Y , Z , ID) VALUES (1," + this.ChankPoint.x + "," + this.ChankPoint.y + "," + this.ChankPoint.z + "," + id + ")";
                    SQLCommand.ExecuteNonQuery();
                }
                SQLConnection.Close();

                List<string> SQLCommandList = new List<string>();


                SQLCommandList.Add("BEGIN TRANSACTION;");
                SQLCommandList.Add("DELETE FROM Blocks WHERE ShankID=" + id + ";");
                foreach (KeyValuePair<Vector3Int, int> block in this.Blocks)
                {
                    SQLCommandList.Add("INSERT INTO Blocks (ShankID , X , Y , Z , ID) VALUES (" + id.ToString() + "," + block.Key.x.ToString() + "," + block.Key.y.ToString() + "," + block.Key.z.ToString() + "," + block.Value.ToString() + ");");
                }
                SQLCommandList.Add("COMMIT;");

                SQLRuner CommandRuner = new SQLRuner(SQLCommandList);
                RunerThread = new Thread(new ThreadStart(CommandRuner.SendLoop));
                RunerThread.IsBackground = true;
                RunerThread.Start();
            }
        }

        /*
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
        //--Глобальные--
        public static bool Spawn(Vector3Int point) //Показ меша
        {
            return Alphabet[point].Spawn();
        }

        public bool Spawn()
        {
            if (IsBuilded)
            {
                MyObject = GameObject.Instantiate(GameObject) as GameObject;
                MyObject.transform.position = this.ChankPoint;

                Mesh MyMesh = new Mesh();
                MyMesh.vertices = Geometry.VerticleList.ToArray();
                MyMesh.triangles = Geometry.TriangleList.ToArray();
                MyMesh.uv = Geometry.IDList.ToArray();
                MyMesh.uv2 = Geometry.UVList.ToArray();

                MyMesh.RecalculateBounds();
                MyMesh.RecalculateNormals();

                MyObject.GetComponent<MeshFilter>().sharedMesh = MyMesh;
                MyObject.GetComponent<MeshCollider>().sharedMesh = MyMesh;
                Debug.Log("Отрендерен чанк:" + ChankPoint);
                return true;
            }
            return false;
        }

        private void LoadNearbyChanks()
        {
            if (!Alphabet.ContainsKey(ChankPoint + (Vector3Int.forward * 20)))
            {
                AddChank(ChankPoint + (Vector3Int.forward * 20));
            }
            if (!Alphabet.ContainsKey(ChankPoint + (Vector3Int.back * 20)))
            {
                AddChank(ChankPoint + (Vector3Int.back * 20));
            }
            if (!Alphabet.ContainsKey(ChankPoint + (Vector3Int.left * 20)))
            {
                AddChank(ChankPoint + (Vector3Int.left * 20));
            }
            if (!Alphabet.ContainsKey(ChankPoint + (Vector3Int.right * 20)))
            {
                AddChank(ChankPoint + (Vector3Int.right * 20));
            }
        }

        public void Show()
        {
            MyObject.SetActive(true);
        }

        public void Hide()
        {
            MyObject.SetActive(false);
        }



        //--Приватные--
        public int GetLocalBlockID(Vector3Int BlockPoint)
        {
            bool Plag;
            return GetLocalBlockID(BlockPoint, out Plag);
        }

        private int GetLocalBlockID(Vector3Int BlockPoint, out bool IsSide)
        {
            Vector3Int OtherChankPoint = this.ChankPoint;
            Vector3Int OtheBlockPoint = BlockPoint;
            int BlockID = 0;
            IsSide = false;

            if (BlockPoint.x < 0)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.left * 20);
                OtheBlockPoint += Vector3Int.right * 20;
                IsSide = true;
            }
            if (19 < BlockPoint.x)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.right * 20);
                OtheBlockPoint += Vector3Int.left * 20;
                IsSide = true;
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
                IsSide = true;
            }
            if (19 < BlockPoint.z)
            {
                OtherChankPoint = this.ChankPoint + (Vector3Int.forward * 20);
                OtheBlockPoint += Vector3Int.back * 20;
                IsSide = true;
            }

            if (Alphabet.ContainsKey(OtherChankPoint))
            {
                Alphabet[OtherChankPoint].Blocks.TryGetValue(OtheBlockPoint, out BlockID);
            }
            return BlockID;
        }
        /*
        public void SetBlock(Vector3Int point, int id)
        {
            if (!Blocks.ContainsKey(point))
            {
                IsModifyed = true;
                Blocks.Add(point, id);
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
            if (Blocks.ContainsKey(point))
            {
                IsModifyed = true;
                //AddBlockMash(point, id);
                List<int> NearBlocksSides = CutAllBlockMash(point);
                Blocks.Remove(point);

                int OldVerticlesCount = Verticles.Count;
                int OldTrianglesCount = Triangles.Count;
                int ID;

                bool isSide;
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
                            ID = GetLocalBlockID(point + Vector3Int.forward, out isSide);
                            if (ID != 0)
                            {
                                AddBackSideMesh(point + Vector3Int.forward, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 4:
                            ID = GetLocalBlockID(point + Vector3Int.back, out isSide);
                            if (ID != 0)
                            {
                                AddForwardSideMesh(point + Vector3Int.back, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 5:
                            ID = GetLocalBlockID(point + Vector3Int.left, out isSide);
                            if (ID != 0)
                            {
                                AddRightSideMesh(point + Vector3Int.left, ID, ref OldVerticlesCount, ref OldTrianglesCount);
                            }
                            break;
                        case 6:
                            ID = GetLocalBlockID(point + Vector3Int.right, out isSide);
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
            if (Blocks.ContainsKey(point))
            {
                bool Transparency = Block.IsTransparency(GetLocalBlockID(point));
                List<MyMeshInfo> Pointers = StartMeshPointers.FindAll(pointer => (pointer.BlockPoint == point));
                foreach (MyMeshInfo Pointer in Pointers)
                {
                    if (!Transparency)
                        NearBlocksSides.Remove(Pointer.SideID);
                    CutSideBlockMash(Pointer.BlockPoint, Pointer.SideID);
                }
            }
            return NearBlocksSides;
        }

        private void CutSideBlockMash(Vector3Int point, int sideID)
        {
            if (Blocks.ContainsKey(point) | (point.x < 0) | (point.y < 0) | (point.x > 20) | ((point.y > 20)))
            {
                MyMeshInfo CurrInfo = StartMeshPointers.Find(pointer => (pointer.BlockPoint == point) & (pointer.SideID == sideID));
                if (CurrInfo.SideID != 0)
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
        */
    }
 

    public class SQLRuner
    {
        List<string> SQLommandTextList;

        public SQLRuner(List<string> sQLommandTextList)
        {
            SQLommandTextList = sQLommandTextList;
        }

        public void SendLoop()
        {
            if (SQLommandTextList != null)
            {
                Debug.Log("SQL-ранер стартовал(" + SQLommandTextList.Count + ")");
                string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
                string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
                SqliteConnection SQLConnection = new SqliteConnection(Params);
                SqliteCommand SQLComand = SQLConnection.CreateCommand();
                SQLConnection.Open();

                foreach (string SQLommandText in SQLommandTextList)
                {
                    SQLComand.CommandText = SQLommandText;
                    SQLComand.ExecuteNonQuery();
                }
                Debug.Log("SQL-ранер остановился");
            }

        }
    }


}

