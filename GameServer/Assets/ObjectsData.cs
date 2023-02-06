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
    public class BlocksIDData
    {
        [SerializeField]
        public Vector3Int Point;
        [SerializeField]
        public int ID;

        BlocksIDData() { }

        public BlocksIDData(Vector3Int point, int id)
        {
            Point = point;
            ID = id;
        }
    }

    [Serializable]
    public class ChankData
    {
        [SerializeField]
        public List<BlocksIDData> BlocksID;
        [SerializeField]
        public Vector3 Point;

        public ChankData() { }

        public ChankData(Vector3 point, Dictionary<Vector3Int, int> blocksID)
        {
            BlocksID = new List<BlocksIDData>();
            Point = point;
            foreach (KeyValuePair<Vector3Int, int> blockID in blocksID)
            {
                BlocksID.Add(new BlocksIDData(blockID.Key, blockID.Value));
            }
        }
    }

    [Serializable]
    public class ChankDataList
    {
        [SerializeField]
        public List<ChankData> Chanks;

        public ChankDataList() { }

        public ChankDataList(Dictionary<Vector3, Chank> chanks)
        {
            Chanks = new List<ChankData>();

            foreach (KeyValuePair<Vector3, Chank> chank in chanks)
            {
                ChankData chankData = new ChankData(chank.Key, chank.Value.BlocksID);
                if(chankData.BlocksID.Count > 0)
                { 
                    Chanks.Add(chankData);
                }
            }
        }
    }
}