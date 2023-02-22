using UnityEngine;
using MyStruct;

namespace Workers
{
    public class ChunkToFactoryAdder
    {
        public ChunkPos CurrChankPoint;
        private ChunkPos AddPoint = new ChunkPos();

        public ChunkToFactoryAdder(ChunkPos �urrChankPoint)
        {
            CurrChankPoint = �urrChankPoint;
        }

        public void AdderLoop()
        {
            Debug.Log("���������� ������ � ������� ��������");
            // Pfuheprf
            AddPoint.z = -4 * 20;
            for (int x = -4; x < 4; x++)
            {
                AddPoint.x = x * 20;
                ChunkLoaderFactory.AddToNeadLoadList(CurrChankPoint + AddPoint);

            }
            AddPoint.z = 4 * 20;
            for (int x = -4; x < 4; x++)
            {
                AddPoint.x = x * 20;
                ChunkLoaderFactory.AddToNeadLoadList(CurrChankPoint + AddPoint);

            }
            AddPoint.x = -4 * 20;
            for (int z = -4; z < 4; z++)
            {
                AddPoint.z = z * 20;
                ChunkLoaderFactory.AddToNeadLoadList(CurrChankPoint + AddPoint);
            }
            AddPoint.x = 4 * 20;
            for (int z = -4; z < 4; z++)
            {
                AddPoint.z = z * 20;
                ChunkLoaderFactory.AddToNeadLoadList(CurrChankPoint + AddPoint);
            }
            // ���������
            AddPoint.z = -40;
            for (int x = -2; x < 2; x++)
            {
                AddPoint.x = x * 20;
                 ChunkBuilderFactory.AddToNeadBuildList(CurrChankPoint + AddPoint);
                
            }
            AddPoint.x = -40;
            for (int z = -2; z < 2; z++)
            {
                AddPoint.z = z * 20;
                 ChunkBuilderFactory.AddToNeadBuildList(CurrChankPoint + AddPoint);

            }
            AddPoint.z = 40;
            for (int x = -2; x < 2; x++)
            {
                AddPoint.x = x * 20;
                ChunkBuilderFactory.AddToNeadBuildList(CurrChankPoint + AddPoint);

            }
            AddPoint.x = 40;
            for (int z = -2; z < 2; z++)
            {
                AddPoint.z = z * 20;
                ChunkBuilderFactory.AddToNeadBuildList(CurrChankPoint + AddPoint);

            }
            // �����
            AddPoint.z = -20;
            for (int x = -1; x < 1; x++)
            {
                AddPoint.x = x * 20;
                ChunkSpawner.AddToNeadSpawnList(CurrChankPoint + AddPoint);
                
            }
            AddPoint.x = -20;
            for (int z = -1; z < 1; z++)
            {
                AddPoint.z = z * 20;
                ChunkSpawner.AddToNeadSpawnList(CurrChankPoint + AddPoint);

            }
            AddPoint.z = 20;
            for (int x = -1; x < 1; x++)
            {
                AddPoint.x = x * 20;
                ChunkSpawner.AddToNeadSpawnList(CurrChankPoint + AddPoint);

            }
            AddPoint.x = 20;
            for (int z = -1; z < 1; z++)
            {
                AddPoint.z = z * 20;
                ChunkSpawner.AddToNeadSpawnList(CurrChankPoint + AddPoint);

            }

            Debug.Log("���������� ������ � ������� ������������");
        }
    }

    public class AreaChunkToFactoryAdder
    {
        public ChunkPos CurrChankPoint;
        private ChunkPos AddPoint = new ChunkPos();

        public AreaChunkToFactoryAdder(ChunkPos �urrChankPoint)
        {
            CurrChankPoint = �urrChankPoint;
        }

        public void AdderLoop()
        {
            Debug.Log("���������� ������� ������ � ������� ��������");
            // ��������
            for (int x = -6; x < 6; x++)
            {
                for (int z = -6; z < 6; z++)
                {
                    AddPoint.x = x * 20;
                    AddPoint.z = z * 20;
                    ChunkLoaderFactory.AddToNeadLoadList(CurrChankPoint + AddPoint);
                }
            }
            //Thread.Sleep(2000);

            // ���������
            for (int x = -4; x < 4; x++)
            {
                for (int z = -4; z < 4; z++)
                {
                    AddPoint.x = x * 20;
                    AddPoint.z = z * 20;

                    ChunkBuilderFactory.AddToNeadBuildList(CurrChankPoint + AddPoint);
                }
            }
            //Thread.Sleep(5000);

            // �����
            for (int x = -3; x < 3; x++)
            {
                for (int z = -3; z < 3; z++)
                {
                    AddPoint.z = z * 20;
                    AddPoint.x = x * 20;
                    ChunkSpawner.AddToNeadSpawnList(CurrChankPoint + AddPoint);
                    
                }
            }
            //Thread.Sleep(1000);

            Debug.Log("���������� ������� ������ � ������� ������������");
        }
    }
}