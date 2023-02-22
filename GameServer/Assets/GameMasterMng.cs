using BaseObjects;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Net.NetworkInformation;
using ObjectsData;
using System.Runtime.Serialization.Formatters.Binary;
using BaseCreature;
using MyStruct;

public class GameMasterMng : MonoBehaviour
{
    

    public bool IsRenderOne;
    public bool IsGenerateOne;
    public bool IsLoadOne;

    public bool SQLOk;
    public bool IsSetBlock; 
    public bool IsDeleteBlock;

    public int X;
    public int Z;

    public int Sub_X;
    public int Sub_Y;
    public int Sub_Z;

    private static List<int> PlayersNeedToSpawnList = new List<int>();
    
    private static List<Player> PlayersList = new List<Player>();


    public static void AddPlayer(string ip)
    {
        PlayersList.Add(new Player(ip, PlayersList.Count * 4 + 1, PlayersList.Count));
        PlayersNeedToSpawnList.Add(PlayersList.Count-1);
    }

    // Start is called before the first frame update
    void Start()
    {
        Block.Load();        
        Application.targetFrameRate = 45;
        InitializationSQLDataBase();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayersNeedToSpawnList.Count  > 0)
        {
            Chunk CurrChunk;
            if (Chunk.Alphabet.TryGetValue(PlayersList[PlayersNeedToSpawnList[0]].CurrChank, out CurrChunk))
            {
                if (CurrChunk.IsSpawn)
                {
                    PlayersList[PlayersNeedToSpawnList[0]].Spawn();
                }
                else
                {
                    PlayersNeedToSpawnList.Add(PlayersNeedToSpawnList[0]);
                }
                PlayersNeedToSpawnList.RemoveAt(0);
            }
        }

        
        if (IsRenderOne)
        {
            IsRenderOne = false;
            ChunkSpawner.AddToNeadSpawnList(new ChunkPos(X * 20, Z * 20));

        }

        if (IsGenerateOne)
        {
            IsGenerateOne = false;
            ChunkBuilderFactory.AddToNeadBuildList(new ChunkPos(X * 20, Z * 20));
        }

        if (IsLoadOne)
        {
            IsLoadOne = false;
            ChunkLoaderFactory.AddToNeadLoadList(new ChunkPos(X * 20, Z * 20));
        }

        if (IsSetBlock)
        {
            IsSetBlock = false;
        }

        if (IsDeleteBlock)
        {
            IsDeleteBlock = false;
        }
    }

    private void OnDestroy()
    {
        Block.Save();
    }

    private void InitializationSQLDataBase()
    {
        string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData";


        if (!Directory.Exists(StorePath))
        {
            Directory.CreateDirectory(StorePath);
        }

        StorePath += @"\GameData.db";
        string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
        SqliteConnection SQLConnection = new SqliteConnection(Params);
        SqliteCommand SQLComand = SQLConnection.CreateCommand();
        SQLConnection.Open();
        SQLComand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Worlds';";
        SqliteDataReader Reader = SQLComand.ExecuteReader();
        if (Reader.HasRows)
        {
            SQLOk = true;
            return;
        }
        else
        {
            Reader.Close();
            SQLComand.CommandText = "CREATE TABLE Worlds (Name TEXT[30], ID INT NOT NULL, PRIMARY KEY (ID));";
            SQLComand.ExecuteNonQuery();
            SQLComand.CommandText = "CREATE TABLE Shanks (WorldID INT, X INT, Z INT, ID INT, PRIMARY KEY (WorldID, X, Z), CONSTRAINT FKWorldID FOREIGN KEY (WorldID) REFERENCES Worlds(ID) ON DELETE CASCADE);";
            SQLComand.ExecuteNonQuery();
            SQLComand.CommandText = "CREATE TABLE Blocks (ShankID INT, X SMALLINT, Y INT, Z SMALLINT, ID INT, PRIMARY KEY (ShankID, X, Y, Z), CONSTRAINT FKChankID FOREIGN KEY (ShankID) REFERENCES Shanks(ID) ON DELETE CASCADE);";
            SQLComand.ExecuteNonQuery();
            SQLComand.CommandText = "CREATE TABLE Clients (IP String, X FLOAT, Y FLOAT, Z FLOAT, Name TEXT[30], PRIMARY KEY (IP));";
            SQLComand.ExecuteNonQuery();
            SQLOk = true;
        }

        SQLConnection.Close();

    }

    private void RunPackaging()
    {
        string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.bin";
        if (File.Exists(StorePath))
        {
            File.Delete(StorePath);
        }

        FileStream NewFile = new FileStream(StorePath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(NewFile, new ChankDataList(Chunk.Alphabet));
        NewFile.Close();
    }

    private void RunUnpacking()
    {
        string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.bin";
        if (File.Exists(StorePath))
        {
            FileStream OldFile = new FileStream(StorePath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            ChankDataList DataList = (ChankDataList)formatter.Deserialize(OldFile);
            foreach (ChankData chank in DataList.Chanks)
            {
                Chunk.AddChank(new ChunkPos(chank.Point_x, chank.Point_z), chank.BlocksID);
            }
            OldFile.Close();
        }
    }

    private void RunSave()
    {
        string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
        string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
        SqliteConnection SQLConnection = new SqliteConnection(Params);
        SqliteCommand SQLComand = SQLConnection.CreateCommand();
        SQLConnection.Open();
        // - мир
        SQLComand.CommandText = "SELECT Name FROM Worlds WHERE Name='Default';";
        SqliteDataReader Reader = SQLComand.ExecuteReader();
        if (!Reader.HasRows)
        {
            Reader.Close();
            SQLComand.CommandText = "INSERT INTO Worlds (Name, ID) VALUES ('Default',1);";
            SQLComand.ExecuteNonQuery();
        }
        else Reader.Close();
        // - чанки и блоки
        SQLComand.CommandText = "DELETE FROM Shanks WHERE WorldID='1';";
        SQLComand.ExecuteNonQuery();
        int id = 1;

        SQLComand.CommandText = "BEGIN TRANSACTION";
        SQLComand.ExecuteNonQuery();
        foreach (KeyValuePair<ChunkPos, Chunk> chank in Chunk.Alphabet)
        {
            if (chank.Value.Blocks.Count > 0) // Если в чанке есть блоки
            {
                SQLComand.CommandText = "INSERT INTO Shanks (WorldID , X, Z , ID) VALUES (1," + chank.Key.x.ToString() + "," + chank.Key.z.ToString() + "," + id.ToString() + ")";
                SQLComand.ExecuteNonQuery();
                SQLComand.CommandText = "DELETE FROM Blocks WHERE ShankID=" + id.ToString();
                SQLComand.ExecuteNonQuery();

                foreach (KeyValuePair<BlockPos, int> block in chank.Value.Blocks)
                {
                    SQLComand.CommandText = "INSERT INTO Blocks (ShankID , X , Y , Z , ID) VALUES (" + id.ToString() + "," + block.Key.x.ToString() + "," + block.Key.y.ToString() + "," + block.Key.z.ToString() + "," + block.Value.ToString() + ")";
                    SQLComand.ExecuteNonQuery();
                }

                id++;

                if(id%100 == 0)
                {
                    SQLComand.CommandText = "COMMIT; BEGIN TRANSACTION";
                    SQLComand.ExecuteNonQuery();
                }
            }
        }
        SQLComand.CommandText = "COMMIT;";
        SQLComand.ExecuteNonQuery();

        SQLConnection.Close();
    }
    /*
    private void RunLoad()
    {
        string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MyServerData\GameData.db";
        string Params = "Data Source=" + StorePath + ";Foreign Keys = true";
        SqliteConnection SQLConnection = new SqliteConnection(Params);
        SqliteCommand SQLComand1 = SQLConnection.CreateCommand();
        SqliteCommand SQLComand2 = SQLConnection.CreateCommand();
        SQLConnection.Open();
        // чанки иблоки
        SQLComand1.CommandText = "SELECT X , Y , Z , ID FROM Shanks WHERE WorldID = 1;";
        SqliteDataReader Reader1 = SQLComand1.ExecuteReader();
        if (!Reader1.HasRows) // - мир пустой или не найден
        {
            Reader1.Close();
            return;
        }
        else
        {
            while (Reader1.Read())
            {
                Dictionary<Vector3Int, int> LoadBloksID = new Dictionary<Vector3Int, int>();
                SQLComand2.CommandText = "SELECT X , Y , Z , ID FROM Blocks WHERE ShankID = " + Reader1["ID"] + ";";
                SqliteDataReader Reader2 = SQLComand2.ExecuteReader();

                if (!Reader2.HasRows) // - чанк пустой берем смледующий
                {
                    Reader2.Close();
                    continue;
                }
                else // -- Если блоки в чанке есть грузим
                {
                    while (Reader2.Read())
                    {

                        LoadBloksID.Add(new Vector3Int(Reader2.GetInt32("X"), Reader2.GetInt32("Y"), Reader2.GetInt32("Z")), Reader2.GetInt32("ID"));
                    }
                    Reader2.Close();
                }
                Chank.AddChank(new Vector3(Reader1.GetFloat("X"), Reader1.GetFloat("Y"), Reader1.GetFloat("Z")), LoadBloksID);
            }

            Reader1.Close();
        }

        SQLConnection.Close();
    }
    */
}
