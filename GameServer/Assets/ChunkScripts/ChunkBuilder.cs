using BaseObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workers
{
   

    public class ChunkBuilder
    {
        // --Чанк--
        Chunk CurrChunk;
        Vector3Int ChunkPoint;

        // --Геометрия--
        ChunkGeometry NewGeometry;

        public ChunkBuilder(Vector3Int chankPoint)
        {
            ChunkPoint = chankPoint;
        }

        public void BuildLoop()
        {
            Build(ChunkPoint);
            ChunkSpawner.AddToReadySpawnList(ChunkPoint);
        }
    

        private void Build(Vector3Int chankPoint)
        {
            CurrChunk = Chunk.Alphabet[chankPoint];
            if (CurrChunk.IsLoaded)
            {
                CurrChunk.IsBuild = true;
                NewGeometry = new ChunkGeometry();
                foreach (KeyValuePair<Vector3Int, int> CurrBlock in CurrChunk.Blocks)
                {
                    AddBlockMash(CurrBlock.Key, CurrBlock.Value);
                }
                CurrChunk.Geometry = NewGeometry;
                CurrChunk.IsBuilded = true;
            }
        }

        private void AddBlockMash(Vector3Int point, int id)
        {
            {
                int OldVerticlesCount = NewGeometry.VerticleList.Count;
                int OldTrianglesCount = NewGeometry.TriangleList.Count;

                AddUpSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

                AddDownSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

                AddForwardSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

                AddBackSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

                AddLeftSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);

                AddRightSideMesh(point, id, ref OldVerticlesCount, ref OldTrianglesCount);
            }
        }

        private void AddRightSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + Vector3Int.right))) //6
            {
                NewGeometry.VerticleList.Add(point + new Vector3(1, 0, 0)); //0
                NewGeometry.VerticleList.Add(point + new Vector3(1, 0, 1)); //1
                NewGeometry.VerticleList.Add(point + new Vector3(1, 1, 0)); //2
                NewGeometry.VerticleList.Add(point + new Vector3(1, 1, 1)); //3
                AddPositivUV();

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 6));

                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddLeftSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + Vector3Int.left))) //5
            {
                NewGeometry.VerticleList.Add(point + new Vector3(0, 0, 0)); //0
                NewGeometry.VerticleList.Add(point + new Vector3(0, 1, 0)); //2
                NewGeometry.VerticleList.Add(point + new Vector3(0, 0, 1)); //1
                NewGeometry.VerticleList.Add(point + new Vector3(0, 1, 1)); //3
                AddNegativUV();

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);

                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 5));

                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddBackSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + Vector3Int.back))) //4
            {
                NewGeometry.VerticleList.Add(point + new Vector3(0, 0, 0)); //0
                NewGeometry.VerticleList.Add(point + new Vector3(1, 0, 0)); //1
                NewGeometry.VerticleList.Add(point + new Vector3(0, 1, 0)); //2
                NewGeometry.VerticleList.Add(point + new Vector3(1, 1, 0)); //3
                AddPositivUV();

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);

                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 4));
                
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddForwardSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + Vector3Int.forward))) //3
            {
                NewGeometry.VerticleList.Add(point + new Vector3(0, 0, 1)); //0
                NewGeometry.VerticleList.Add(point + new Vector3(0, 1, 1)); //2
                NewGeometry.VerticleList.Add(point + new Vector3(1, 0, 1)); //1
                NewGeometry.VerticleList.Add(point + new Vector3(1, 1, 1)); //3
                AddNegativUV();

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);

                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 3));
                
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddDownSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            bool isSide = false;
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + Vector3Int.down))) //2
            {
                NewGeometry.VerticleList.Add(point + new Vector3(0, 0, 0)); //0
                NewGeometry.VerticleList.Add(point + new Vector3(0, 0, 1)); //2
                NewGeometry.VerticleList.Add(point + new Vector3(1, 0, 0)); //1
                NewGeometry.VerticleList.Add(point + new Vector3(1, 0, 1)); //3
                AddPositivUV();

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 2));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddUpSideMesh(Vector3Int point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + Vector3Int.up)))  //1
            {
                NewGeometry.VerticleList.Add(point + new Vector3(0, 1, 0)); //0
                NewGeometry.VerticleList.Add(point + new Vector3(1, 1, 0)); //1
                NewGeometry.VerticleList.Add(point + new Vector3(0, 1, 1)); //2
                NewGeometry.VerticleList.Add(point + new Vector3(1, 1, 1)); //3
                AddPositivUV();

                AddDataInUV(id);
                AddTriengles(OldVerticlesCount);
                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 1));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddPositivUV()
        {
            NewGeometry.UVList.Add(new Vector2(0, 0));
            NewGeometry.UVList.Add(new Vector2(1, 0));
            NewGeometry.UVList.Add(new Vector2(0, 1));
            NewGeometry.UVList.Add(new Vector2(1, 1));
        }

        private void AddNegativUV()
        {
            NewGeometry.UVList.Add(new Vector2(0, 0));
            NewGeometry.UVList.Add(new Vector2(0, 1));
            NewGeometry.UVList.Add(new Vector2(1, 0));
            NewGeometry.UVList.Add(new Vector2(1, 1));
        }

        private void AddDataInUV(int blockID)
        {
            NewGeometry.IDList.Add(new Vector2(blockID, 0));
            NewGeometry.IDList.Add(new Vector2(blockID, 0));
            NewGeometry.IDList.Add(new Vector2(blockID, 0));
            NewGeometry.IDList.Add(new Vector2(blockID, 0));
        }

        private void AddTriengles(int oldCount)
        {
            NewGeometry.TriangleList.Add(oldCount + 0);
            NewGeometry.TriangleList.Add(oldCount + 2);
            NewGeometry.TriangleList.Add(oldCount + 1);

            NewGeometry.TriangleList.Add(oldCount + 3);
            NewGeometry.TriangleList.Add(oldCount + 1);
            NewGeometry.TriangleList.Add(oldCount + 2);
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

    public struct ChunkGeometry
    {
        public List<Vector3> VerticleList;
        public List<int> TriangleList;
        public List<Vector2> IDList;
        public List<Vector2> UVList;
        public List<MyMeshInfo> MeshInfoList;

        public ChunkGeometry(bool Chop)
        {
            VerticleList = new List<Vector3>();
            TriangleList = new List<int>();
            IDList = new List<Vector2>();
            UVList = new List<Vector2>();
            MeshInfoList = new List<MyMeshInfo>();
        }
    }
}


