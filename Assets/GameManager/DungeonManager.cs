﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;
    public List<PossibleChallengeData> possibleChallenges;
    public Dictionary<TypeOfEnemy, int> tierOfEnemies = new Dictionary<TypeOfEnemy, int>();

    [SerializeField] private int numberOfChallengesToGenerate;
    [SerializeField] private GameObject[] roomToSpawnNextPrefab;
    [SerializeField] private Transform roomsHolder;
    [SerializeField] private Transform enemyBulletHolder;
    [SerializeField] private List<RoomManager> allRooms;
    [SerializeField] private int globalTier = 0;

    private int roomID = -1;
    private int nextRoomToGenerateIndex = 0;
    private ScoreManager scoreManager;
    private bool firstTimeGeneratingChallenges = true;
    

    #region getters
    public int GetGlobalTier() { return globalTier; }
    public Transform GetBulletHolder() { return enemyBulletHolder; }
    public int GetNumberOfRoomsToBeatDungeon() { return scoreManager.GetNumberOfRoomsToBeatDungeon(); }
    public int GetRoomID(){ return roomID;}
    public bool GetFirstTimeGeneratingChallenges(){ return firstTimeGeneratingChallenges;}
    public List<RoomManager> GetAllRooms(){ return allRooms;}
    #endregion

    private void Awake()
    {
        MakeThisObjectSingleton();
        
        scoreManager = GetComponent<ScoreManager>();
    }
    
    private void Start()
    {
        InitializePossibleChallengesList();
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
        allRooms[0].RoomCleared += GenerateChallengeForNextRoom;
        nextRoomToGenerateIndex = 1;
    }

    private void GenerateChallengeForNextRoom()
    {
        if (nextRoomToGenerateIndex <= GetComponent<ScoreManager>().GetNumberOfRoomsToBeatDungeon())
        {
            CreateNextRoom();
        }
        allRooms[nextRoomToGenerateIndex].GenerateChallengeForThisRoom();
        allRooms[nextRoomToGenerateIndex].RoomCleared += GenerateChallengeForNextRoom;
        allRooms[nextRoomToGenerateIndex].RoomCleared += scoreManager.UpdateScore;

        nextRoomToGenerateIndex++;
        firstTimeGeneratingChallenges = false;
    }

    internal RoomManager GetRoomManagerByRoomID(int roomID)
    {
        foreach(RoomManager roomManager in allRooms)
        {
            if(roomManager.GetRoomID() == roomID)
            {
                return roomManager;
            }
        }
        Debug.LogError("previous roomManager was not found, can't hide challenge");
        return allRooms[0];
    }

    private void InitializePossibleChallengesList()
    {
        var enemiesCount = EnemyLibrary.instance.GetAllPossibleEnemies().Count;
        for (int i = enemiesCount; i > 0 ; i--)
        {
            numberOfChallengesToGenerate += i;
        }
        numberOfChallengesToGenerate -= enemiesCount;
        possibleChallenges = new List<PossibleChallengeData>
        {
            Capacity = numberOfChallengesToGenerate
        };
        for (int i = 0; i < possibleChallenges.Capacity; i++)
        {
            possibleChallenges.Add(new PossibleChallengeData());
        }
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
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            roomsHolder = GameObject.FindGameObjectWithTag("Dungeon").transform;
            allRooms = new List<RoomManager>();
            allRooms.Add(GameObject.FindGameObjectWithTag("FirstRoom").GetComponent<RoomManager>());
            enemyBulletHolder = GameObject.FindGameObjectWithTag("BulletHolder").transform;

            roomID = -1;
            nextRoomToGenerateIndex = 0;

            CreateNextRoom();
            GenerateChallengeForFirstRoom();
            
        }
    }

    public void IncreaseGlobalTier()
    {
        globalTier++;
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
