using BaseObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectsData
{
    [Serializable]
    public class BlockData
    {
        [SerializeField]
        public int ID;
        [SerializeField]
        public string Name;
        [SerializeField]
        public bool Transparency;

        public BlockData() { }
        public BlockData(int id, string name, bool transparency)
        {
            ID = id;
            Name = name;
            Transparency = transparency;
        }
    }

    [Serializable]
    public class BlocksDataList
    {
        [SerializeField]
        public List<BlockData> Blocks;

        public BlocksDataList() { }
        public BlocksDataList(Dictionary<int, Block> blocks)
        {
            Blocks = new List<BlockData>();
            foreach (KeyValuePair<int, Block> block in blocks)
            {
                Blocks.Add(new BlockData(block.Key, block.Value.Name, block.Value.Transparency));
            }
        }
    }

    [Serializable]
    public struct BlocksIDData
    {
        [SerializeField]
        public int Point_x;
        [SerializeField]
        public int Point_y;
        [SerializeField]
        public int Point_z;
        [SerializeField]
        public int ID;

        public BlocksIDData(Vector3Int point, int id)
        {
            Point_x = point.x;
            Point_y = point.y;
            Point_z = point.z;
            ID = id;
        }
    }

    [Serializable]
    public struct ChankData
    {
        [SerializeField]
        //public int[,,] BlockIDArrey;
        public List<BlocksIDData> BlocksID;
        [SerializeField]
        public int Point_x;
        [SerializeField]
        public int Point_y;
        [SerializeField]
        public int Point_z;


        public ChankData(Vector3Int point, Dictionary<Vector3Int, int> blocksID)
        {
            BlocksID = new List<BlocksIDData>();
            Point_x = point.x;
            Point_y = point.y;
            Point_z = point.z;
            //BlockIDArrey = new int[20,20,20];
            foreach (KeyValuePair<Vector3Int, int> blockID in blocksID)
            {
                //BlockIDArrey[blockID.Key.x, blockID.Key.y, blockID.Key.z] = blockID.Value;
                BlocksID.Add(new BlocksIDData(blockID.Key, blockID.Value));
            }
        }
    }

    [Serializable]
    public struct ChankDataList
    {
        [SerializeField]
        public List<ChankData> Chanks;

        public ChankDataList(Dictionary<Vector3Int, Chank> chanks)
        {
            Chanks = new List<ChankData>();

            foreach (KeyValuePair<Vector3Int, Chank> chank in chanks)
            {
                ChankData chankData = new ChankData(chank.Key, chank.Value.BlocksID);
                //if(chankData.BlocksID.Count > 0)
                //{ 
                    Chanks.Add(chankData);
                //}
            }
        }
    }
}