using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;
    //public List<PossibleChallengeData> possibleChallenges;

    public Dictionary<TypeOfEnemy, int> tierOfEnemies = new Dictionary<TypeOfEnemy, int>();

    [SerializeField] private int numberOfChallengesToGenerate;
    [SerializeField] private GameObject[] roomToSpawnNextPrefab;
    [SerializeField] private Transform roomsHolder;
    [SerializeField] private Transform enemyBulletHolder;
    [SerializeField] private List<RoomManager> allRooms;
    public List<TypeOfEnemy[]> skipedChallenges = new List<TypeOfEnemy[]>();

    private int roomID = -1;
    private int nextRoomToGenerateIndex = 0;
    private ScoreManager scoreManager;
    private bool firstTimeGeneratingChallenges = true;
    public int playersRoom = -1;
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
        //InitializePossibleChallengesList();
        InitializeMonstersTier();
        CreateNextRoom();
        GenerateChallengeForFirstRoom();
        SceneManager.sceneLoaded += SceneLoaded;
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
            allRooms[nextRoomToGenerateIndex].RoomCleared += scoreManager.UpdateScore;
            AfterDeathOptions.instance.OnSkip += scoreManager.UpdateScore;

            nextRoomToGenerateIndex++;
            firstTimeGeneratingChallenges = false;
        }
    }

    public bool DungeonBeaten()
    {
        foreach (var enemy in tierOfEnemies)
        {
            var tierEnemy = enemy.Value;
            foreach (var item in skipedChallenges)
            {
                if (tierEnemy == 4)
                {
                    break;
                }
                if (item[0] == enemy.Key || item[1] == enemy.Key)
                {
                    tierEnemy++;
                }
            }

            if (tierEnemy >= 4)
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
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
    // private void InitializePossibleChallengesList()
    // {
    //     var enemiesCount = EnemyLibrary.instance.GetAllPossibleEnemies().Count;
    //     for (int i = enemiesCount; i > 0 ; i--)
    //     {
    //         numberOfChallengesToGenerate += i;
    //     }
    //     numberOfChallengesToGenerate -= enemiesCount;
    //     possibleChallenges = new List<PossibleChallengeData>
    //     {
    //         Capacity = numberOfChallengesToGenerate
    //     };
    //     for (int i = 0; i < possibleChallenges.Capacity; i++)
    //     {
    //         possibleChallenges.Add(new PossibleChallengeData());
    //     }
    // }

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

            CreateNextRoom();
            GenerateChallengeForFirstRoom();

        }
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
