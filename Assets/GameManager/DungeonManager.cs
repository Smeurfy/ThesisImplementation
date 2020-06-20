using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;

    public Dictionary<TypeOfEnemy, int> tierOfEnemies = new Dictionary<TypeOfEnemy, int>();
    public Dictionary<string, Dictionary<string, MonstersInfo>> monstersInfo = new Dictionary<string, Dictionary<string, MonstersInfo>>();

    [SerializeField] private int numberOfChallengesToGenerate;
    [SerializeField] private GameObject[] roomToSpawnNextPrefab;
    [SerializeField] private Transform roomsHolder;
    [SerializeField] private Transform enemyBulletHolder;
    [SerializeField] private List<RoomManager> allRooms;
    public List<TypeOfEnemy[]> skipedChallenges = new List<TypeOfEnemy[]>();
    public List<PossibleChallengeData> _finalChallenges = new List<PossibleChallengeData>();
    public int _maxDifBewTiers = 1;
    private int roomID = -1;
    private int nextRoomToGenerateIndex = 0;
    private ScoreManager scoreManager;
    private bool firstTimeGeneratingChallenges = true;
    public int playersRoom, indexChallenge = -1;
    private bool canCreateNextRoom = false;


    #region getters
    public Transform GetBulletHolder() { return enemyBulletHolder; }
    public int GetNumberOfRoomsToBeatDungeon() { return scoreManager.GetNumberOfRoomsToBeatDungeon(); }
    public int GetRoomID() { return roomID; }
    public bool GetFirstTimeGeneratingChallenges() { return firstTimeGeneratingChallenges; }
    public List<RoomManager> GetAllRooms() { return allRooms; }
    #endregion

    private void Awake()
    {
        MakeThisObjectSingleton();
        scoreManager = GetComponent<ScoreManager>();

    }

    private void Start()
    {
        LoadFile();
        InitializeMonstersTier();
        GenerateDungeonChallenges();
        CreateNextRoom();
        GenerateChallengeForFirstRoom();
        AfterDeathOptions.instance.OnRestartNewRun += GenerateNewRun;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void GenerateNewRun()
    {
        _finalChallenges.Clear();
        tierOfEnemies.Clear();
        InitializeMonstersTier();
        GenerateDungeonChallenges();
    }

    public void CreateNextRoom()
    {
        var nextRoomPosition = allRooms[nextRoomToGenerateIndex].GetPositionNextRoom();
        SetNextCameraPosition();
        GameObject nextRoom = Instantiate(roomToSpawnNextPrefab[0],
                                          allRooms[nextRoomToGenerateIndex].GetPositionNextRoom(),
                                          Quaternion.identity,
                                          roomsHolder);

        RenameRoom(nextRoom);
        nextRoomPosition.x += 18;
        nextRoom.GetComponentInChildren<RoomManager>().setPositionNextRoom(nextRoomPosition);
        allRooms.Add(nextRoom.GetComponentInChildren<RoomManager>());
    }

    private void GenerateChallengeForFirstRoom()
    {
        allRooms[0].GetDoorHolder().GetComponentInChildren<DoorManager>().OnPlayerSurvivedRemaininBullets += GenerateChallengeForNextRoom;
        nextRoomToGenerateIndex = 1;
    }

    private void GenerateChallengeForNextRoom(bool value)
    {
        if (value)
        {
            // if (!DungeonBeaten())
            // {
            CreateNextRoom();
            // }

            allRooms[nextRoomToGenerateIndex].GenerateChallengeForThisRoom();
            allRooms[nextRoomToGenerateIndex].GetDoorHolder().GetComponentInChildren<DoorManager>().OnPlayerSurvivedRemaininBullets += GenerateChallengeForNextRoom;
            allRooms[nextRoomToGenerateIndex].GetDoorHolder().GetComponentInChildren<DoorManager>().OnPlayerSurvivedRemaininBullets += scoreManager.UpdateScore;
            allRooms[nextRoomToGenerateIndex].RoomCleared += scoreManager.UpdateScoreBeforeExitRoom;
            nextRoomToGenerateIndex++;
            firstTimeGeneratingChallenges = false;
        }
    }

    public bool DungeonBeaten()
    {
        int count = 0;
        foreach (var item in scoreManager._victoryAndLoses)
        {
            if (item == 0 || item == 1)
                count++;
        }
        if (count == 25)
            return true;
        return false;
    }

    internal RoomManager GetRoomManagerByRoomID(int roomID)
    {
        foreach (RoomManager roomManager in allRooms)
        {
            if (roomManager.GetRoomID() == roomID)
            {
                return roomManager;
            }
        }
        //Debug.LogError("previous roomManager was not found, can't hide challenge");
        return allRooms[0];
    }

    private void InitializeMonstersTier()
    {
        foreach (var enemy in EnemyLibrary.instance.GetAllPossibleEnemies())
        {
            tierOfEnemies.Add(enemy, 0);
        }

    }
    ///<summary>
    /// Generates the challenges before starting the game.
    ///</summary>
    private void GenerateDungeonChallenges()
    {
        //List with the final challenges

        for (int i = 0; i < 25; i++)
        {
            _finalChallenges.Add(new PossibleChallengeData());
            _finalChallenges[i].GeneratePossibleChallenge();
            tierOfEnemies[_finalChallenges[i].GetTypeOfEnemies()[0]]++;
            tierOfEnemies[_finalChallenges[i].GetTypeOfEnemies()[1]]++;
            // Debug.Log(_finalChallenges[i].GetTypeOfEnemies()[0] + " " + tierOfEnemies[_finalChallenges[i].GetTypeOfEnemies()[0]]);
            // Debug.Log(_finalChallenges[i].GetTypeOfEnemies()[1] + " " + tierOfEnemies[_finalChallenges[i].GetTypeOfEnemies()[1]]);
        }
        // foreach (var item in tierOfEnemies)
        // {
        //     Debug.Log(item.Key.name + " " + item.Value);
        // }
        tierOfEnemies.Clear();
        InitializeMonstersTier();
    }

    internal void AssignRoomID()
    {
        roomID++;
    }

    private void SetNextCameraPosition()
    {
        if (roomID != 0 && CameraLookAtRoom.instance)
        {
            int nextRoomIndex = roomID - 1;
            CameraLookAtRoom.instance.NextRoomsPosition(allRooms[nextRoomIndex].GetPositionNextRoom());
        }
    }

    public void RenameRoom(GameObject nextRoom)
    {
        int nextRoomsID = roomID;
        nextRoom.name = "Room " + nextRoomsID++;

    }

    private void SceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        Debug.Log("scene loaded");

        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            roomsHolder = GameObject.FindGameObjectWithTag("Dungeon").transform;
            allRooms = new List<RoomManager>();
            allRooms.Add(GameObject.FindGameObjectWithTag("FirstRoom").GetComponent<RoomManager>());
            enemyBulletHolder = GameObject.FindGameObjectWithTag("BulletHolder").transform;

            tierOfEnemies.Clear();
            tierOfEnemies = new Dictionary<TypeOfEnemy, int>();
            skipedChallenges = new List<TypeOfEnemy[]>();
            InitializeMonstersTier();
            firstTimeGeneratingChallenges = true;
            roomID = -1;
            nextRoomToGenerateIndex = 0;
            playersRoom = -1;
            indexChallenge = -1;

            CreateNextRoom();
            GenerateChallengeForFirstRoom();
            AfterDeathOptions.instance.OnRestartNewRun += GenerateNewRun;

        }
    }

    ///<summary>
    ///Load the characteristics of the monsters from a file
    ///</summary>
    void LoadFile()
    {
        string path = Application.dataPath + "/StreamingAssets/denis.json";
        string[] fileContent = File.ReadAllLines(path);
        var enemies = EnemyLibrary.instance.GetAllPossibleEnemiesPrefabs();
        foreach (var item in enemies)
        {
            monstersInfo.Add(item.name, new Dictionary<string, MonstersInfo>());
        }
        foreach (var item in monstersInfo)
        {
            foreach (var str in fileContent)
            {
                MonstersInfo obj = JsonUtility.FromJson<MonstersInfo>(str);
                if (obj.monsterName == item.Key)
                {
                    item.Value.Add(obj.tier, obj);
                }
            }
        }
        // foreach (var item in monstersInfo)
        // {
        // 	Debug.Log(item.Key);
        //     foreach (var str in item.Value)
        //     {
        // 		Debug.Log(str.Key);
        //     }
        // }
    }

    private void MakeThisObjectSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
