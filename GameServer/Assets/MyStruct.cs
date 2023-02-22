using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyStruct
{
    public struct BlockPos
    {
        public sbyte x { get; set; }
        public sbyte z { get; set; }
        public int y { get; set; }

        public BlockPos(sbyte newX, int newY, sbyte newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public BlockPos(int newX, int newY, int newZ)
        {
            x = (sbyte)newX;
            y = newY;
            z = (sbyte)newZ;
        }

        public Vector3 GepVector3()
        {
            return new Vector3(x, y, z);
        }

        public static readonly BlockPos zero = new BlockPos(0, 0, 0);

        public static readonly BlockPos one = new BlockPos(1, 1, 1);

        public static readonly BlockPos up = new BlockPos(0, 1, 0);

        public static readonly BlockPos down = new BlockPos(0, -1, 0);

        public static readonly BlockPos left = new BlockPos(-1, 0, 0);

        public static readonly BlockPos right = new BlockPos(1, 0, 0);

        public static readonly BlockPos forward = new BlockPos(0, 0, 1);

        public static readonly BlockPos back = new BlockPos(0, 0, -1);

        public static BlockPos operator *(BlockPos pos, int number)
        {
            return new BlockPos(pos.x * number, pos.y * number, pos.z * number);
        }

        public static BlockPos operator +(BlockPos pos1, BlockPos pos2)
        {
            return new BlockPos(pos1.x + pos2.x, pos1.y + pos2.y, pos1.z + pos2.z);
        }
    }

    public struct ChunkPos
    {
        public int x { get; set; }
        public int z { get; set; }

        public ChunkPos(int newX, int newZ)
        {
            x = newX;
            z = newZ;
        }

        public ChunkPos(Vector3 vector3)
        {
            x = (int)vector3.x;
            z = (int)vector3.z;
        }

        public Vector3 GepVector3()
        {
            return new Vector3(x, 0, z);
        }

        public static ChunkPos VectorToPoss(Vector3 vector3)
        {
            return new ChunkPos(vector3);
        }

        public static readonly ChunkPos zero = new ChunkPos(0, 0);

        public static readonly ChunkPos one = new ChunkPos(1, 1);

        public static readonly ChunkPos left = new ChunkPos(-1, 0);

        public static readonly ChunkPos right = new ChunkPos(1, 0);

        public static readonly ChunkPos forward = new ChunkPos(0, 1);

        public static readonly ChunkPos back = new ChunkPos(0, -1);

        public static ChunkPos operator *(ChunkPos pos, int number)
        {
            return new ChunkPos(pos.x * number, pos.z * number);
        }

        public static ChunkPos operator +(ChunkPos pos1, ChunkPos pos2)
        {
            return new ChunkPos(pos1.x + pos2.x, pos1.z + pos2.z);
        }

        public static ChunkPos operator -(ChunkPos pos1, ChunkPos pos2)
        {
            return new ChunkPos(pos1.x - pos2.x, pos1.z - pos2.z);
        }

        public static bool operator ==(ChunkPos pos1, ChunkPos pos2)
        {
            return (pos1.x == pos2.x) & (pos1.z == pos2.z);
        }

        public static bool operator !=(ChunkPos pos1, ChunkPos pos2)
        {
            return (pos1.x != pos2.x) | (pos1.z != pos2.z);
        }
    }
}
