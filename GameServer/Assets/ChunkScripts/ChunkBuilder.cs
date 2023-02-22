using BaseObjects;
using MyStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workers
{
   

    public class ChunkBuilder
    {
        // --Чанк--
        Chunk CurrChunk;
        ChunkPos ChunkPoint;

        // --Геометрия--
        private ChunkGeometry NewGeometry;

        public ChunkBuilder(ChunkPos chunkPoint)
        {
            ChunkPoint = chunkPoint;
        }

        public void BuildLoop()
        {
            CurrChunk = Chunk.Alphabet[ChunkPoint];
            CurrChunk.IsBuild = true;
            NewGeometry = new ChunkGeometry(true);
            foreach (KeyValuePair<BlockPos, int> CurrBlock in CurrChunk.Blocks)
            {
                AddBlockMash(CurrBlock.Key, CurrBlock.Value);
            }
            CurrChunk.Geometry = NewGeometry;
            CurrChunk.IsBuilded = true;
            Debug.Log("Построен чанк:" + CurrChunk.ChankPoint);
            
        }

        private void AddBlockMash(BlockPos point, int id)
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

        private void AddRightSideMesh(BlockPos point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + BlockPos.right))) //6
            {
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 0, 0)).GepVector3()); //0
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 0, 1)).GepVector3()); //1
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 1, 0)).GepVector3()); //2
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 1, 1)).GepVector3()); //3
                AddTriengles(OldVerticlesCount);
                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 6));

                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddLeftSideMesh(BlockPos point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + BlockPos.left))) //5
            {
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 0, 0)).GepVector3()); //0
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 1, 0)).GepVector3()); //2
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 0, 1)).GepVector3()); //1
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 1, 1)).GepVector3()); //3
                AddTriengles(OldVerticlesCount);

                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 5));

                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddBackSideMesh(BlockPos point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + BlockPos.back))) //4
            {
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 0, 0)).GepVector3()); //0
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 0, 0)).GepVector3()); //1
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 1, 0)).GepVector3()); //2
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 1, 0)).GepVector3()); //3
                AddTriengles(OldVerticlesCount);

                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 4));
                
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddForwardSideMesh(BlockPos point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + BlockPos.forward))) //3
            {
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 0, 1)).GepVector3()); //0
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 1, 1)).GepVector3()); //2
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 0, 1)).GepVector3()); //1
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 1, 1)).GepVector3()); //3
                AddTriengles(OldVerticlesCount);

                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 3));
                
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddDownSideMesh(BlockPos point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            bool isSide = false;
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + BlockPos.down))) //2
            {
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 0, 0)).GepVector3()); //0
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 0, 1)).GepVector3()); //2
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 0, 0)).GepVector3()); //1
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 0, 1)).GepVector3()); //3
                AddTriengles(OldVerticlesCount);
                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 2));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
        }

        private void AddUpSideMesh(BlockPos point, int id, ref int OldVerticlesCount, ref int OldTrianglesCount)
        {
            if (Block.IsTransparency(CurrChunk.GetLocalBlockID(point + BlockPos.up)))  //1
            {
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 1, 0)).GepVector3()); //0
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 1, 0)).GepVector3()); //1
                NewGeometry.VerticleList.Add((point + new BlockPos(0, 1, 1)).GepVector3()); //2
                NewGeometry.VerticleList.Add((point + new BlockPos(1, 1, 1)).GepVector3()); //3
                AddTriengles(OldVerticlesCount);
                NewGeometry.MeshInfoList.Add(new MyMeshInfo(point, OldVerticlesCount, OldTrianglesCount, 1));
                OldVerticlesCount += 4;
                OldTrianglesCount += 6;
            }
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
        public BlockPos BlockPoint;
        public int StartVerticlIndex;
        public int StartTrianglesIndex;
        public int SideID;

        public MyMeshInfo(BlockPos point, int startVerticlesCount, int startTrianglesCount, int sideID) : this()
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
        public List<MyMeshInfo> MeshInfoList;

        public ChunkGeometry(bool Chop)
        {
            VerticleList = new List<Vector3>();
            TriangleList = new List<int>();
            MeshInfoList = new List<MyMeshInfo>();
        }
    }
}


