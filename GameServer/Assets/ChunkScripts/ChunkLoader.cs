using BaseObjects;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Generator;
using MyStruct;

namespace Workers
{
    public class ChunkLoader
    {
        ChunkPos ChunkPoint;
        Chunk CurrChunk;


        public ChunkLoader(ChunkPos chunkPoint)
        {
            ChunkPoint = chunkPoint;
        }

        public void LoadLoop()
        {
            CurrChunk = new Chunk(ChunkPoint);
            CurrChunk.IsLoad = true;
            
            
            string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
            string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
            SqliteConnection SQLConnection = new SqliteConnection(Params);
            SqliteCommand SQLComand = SQLConnection.CreateCommand();
            SQLConnection.Open();
            
            // чанки и блоки
            SQLComand.CommandText = "SELECT ID FROM Shanks WHERE WorldID = 1 AND X=" + ChunkPoint.x + " AND Z=" + ChunkPoint.z + ";";
            SqliteDataReader Reader = SQLComand.ExecuteReader();
            
            if (!Reader.HasRows) // - мир пустой или не найден
            {
                Reader.Close();
                SQLConnection.Close();
                CurrChunk.Blocks = TerrainGenerator.Run(ChunkPoint);
                CurrChunk.IsModifyed = true;
                CurrChunk.IsLoaded = true;
                Chunk.Alphabet.Add(ChunkPoint, CurrChunk);
                return;
            }

            Reader.Read();

            Dictionary<BlockPos, int> LoadBloksID = new Dictionary<BlockPos, int>();
            string CommandText = "SELECT X , Y , Z , ID FROM Blocks WHERE ShankID = " + Reader["ID"] + ";";
            Reader.Close();
            SQLComand.CommandText = CommandText;
            Reader = SQLComand.ExecuteReader();

            if (!Reader.HasRows) // - чанк пустой перегенерируем ?
            {
                Reader.Close();
                SQLConnection.Close();
                CurrChunk.Blocks = TerrainGenerator.Run(ChunkPoint);
                CurrChunk.IsModifyed = true;
                CurrChunk.IsLoaded = true;
                Chunk.Alphabet.Add(ChunkPoint, CurrChunk);
                Debug.Log("Загружен чанк:" + CurrChunk.ChankPoint);
                return;
            }

            while (Reader.Read())
            {
                LoadBloksID.Add(new BlockPos(Reader.GetByte("X"), Reader.GetInt32("Y"), Reader.GetByte("Z")), Reader.GetInt32("ID"));
            }

            Reader.Close();
            SQLConnection.Close();
            CurrChunk.Blocks = LoadBloksID;
            CurrChunk.IsLoaded = true;
            Chunk.Alphabet.Add(ChunkPoint, CurrChunk);
            Debug.Log("Загружен чанк:" + CurrChunk.ChankPoint);
        }

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

