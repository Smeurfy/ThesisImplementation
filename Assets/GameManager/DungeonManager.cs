using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;
    public List<PossibleChallengeData> possibleChallenges;

    //[SerializeField] [Range(0, 1)] private float performanceWeight;
    //[SerializeField] private float noveltyWeight;

    [SerializeField] private int numberOfChallengesToGenerate;
    [SerializeField] private GameObject[] roomToSpawnNextPrefab;
    [SerializeField] private Transform roomsHolder;
    [SerializeField] private Transform enemyBulletHolder;
    [SerializeField] private List<RoomManager> allRooms;
    [SerializeField] private int numberOfMecanics;

    private int roomID = -1;
    private int nextRoomToGenerateIndex = 0;
    private ScoreManager scoreManager;

    #region getters
    //public float GetPerformanceWeight() { return performanceWeight; }
    //public float GetNoveltyWeight() { return noveltyWeight; }
    public Transform GetBulletHolder() { return enemyBulletHolder; }
    public int GetNumberOfRoomsToBeatDungeon() { return scoreManager.GetNumberOfRoomsToBeatDungeon(); }
    #endregion

    private void Awake()
    {
        //noveltyWeight = 1 - performanceWeight;
        MakeThisObjectSingleton();
        InitializePossibleChallengesList();
        scoreManager = GetComponent<ScoreManager>();
    }
    
    private void Start()
    {
        //if(ReadModelValuesFromPlayer.instance)
        //{
        //    SubscribeToModelValuesPicker();
        //}
        CreateNextRoom();
        GenerateChallengeForFirstRoom();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void CreateNextRoom()
    {
        var nextRoomPosition = allRooms[nextRoomToGenerateIndex].GetPositionNextRoom();
        SetNextCameraPosition();
        GameObject nextRoom = Instantiate(roomToSpawnNextPrefab[0],
                                          allRooms[nextRoomToGenerateIndex].GetPositionNextRoom(),
                                          Quaternion.identity,
                                          roomsHolder);

        RenameRoom(nextRoom);
       
        
        if(nextRoomToGenerateIndex % numberOfMecanics < numberOfMecanics)            
        {
            nextRoomPosition.y -= 12;
        }
        else{
            nextRoomPosition.x += 18;
        }
        
        
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
        //print(nextRoomToGenerateIndex + " " + GetComponent<ScoreManager>().GetNumberOfRoomsToBeatDungeon());
        if (nextRoomToGenerateIndex <= GetComponent<ScoreManager>().GetNumberOfRoomsToBeatDungeon())
        {
            CreateNextRoom();
        }
        allRooms[nextRoomToGenerateIndex].GenerateChallengeForThisRoom();
        allRooms[nextRoomToGenerateIndex].RoomCleared += GenerateChallengeForNextRoom;
        allRooms[nextRoomToGenerateIndex].RoomCleared += scoreManager.UpdateScore;
        //allRooms[nextRoomToGenerateIndex].RoomCleared += PerformanceData.instance.UpdateTagPerformanceMedian;
        nextRoomToGenerateIndex++;
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
        possibleChallenges = new List<PossibleChallengeData>
        {
            Capacity = numberOfChallengesToGenerate
        };
        for (int i = 0; i < possibleChallenges.Capacity; i++)
        {
            possibleChallenges.Add(new PossibleChallengeData());
        }
    }

    internal int AssignRoomID()
    {
        roomID++;
        return roomID;
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

    //private void SubscribeToModelValuesPicker()
    //{
    //    ReadModelValuesFromPlayer.instance.OnValuesSubmitted += SetNoveltyAndPerformanceWeight;
    //}

    //private void SetNoveltyAndPerformanceWeight(int newNoveltyWeight, int desiredNovelty)
    //{
    //    print(newNoveltyWeight + " " + desiredNovelty);   //delete
    //    noveltyWeight = (float) newNoveltyWeight / 100;
    //    performanceWeight = 1 - noveltyWeight;
    //    print("novelty weight " + noveltyWeight + " perf wei: " + performanceWeight);

    //    FindObjectOfType<FirstRoom>().GetComponent<NoveltyAndPerformanceFunctions>().SetNoveltyValueByPlayersInput(desiredNovelty);

    //    ReadModelValuesFromPlayer.instance.OnValuesSubmitted -= SetNoveltyAndPerformanceWeight;
    //    Destroy(ReadModelValuesFromPlayer.instance.gameObject);
    //}

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
