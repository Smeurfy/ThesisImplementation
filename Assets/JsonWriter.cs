using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JsonWriter : MonoBehaviour
{
    private class PrintablePerformanceData
    {
        public string enemyTag;
        public List<int> performanceValues;

        public PrintablePerformanceData(string enemyTag) { this.enemyTag = enemyTag; performanceValues = new List<int>(); }
    }

    private enum DocToWrite { tagPerformance, PlayerStats};

    public static JsonWriter instance;
    
    private List<PrintablePerformanceData> performanceDataCollection;
    private string tagPerformanceFile, playerStatsFile;
    private static bool firstTime = true;
    private static float playTime = 0f;
    private static int runCount = 0;
    private static List<int> roomsPerRun;
    private static List<string> challengeHistoryRun;
    private static int playerAssignedNumber = 0;

    private int roomClearedSoFarInThisRun = 0;

    private void Awake()
    {
        MakeThisObjectSingleton();
        performanceDataCollection = new List<PrintablePerformanceData>();
        tagPerformanceFile = Application.dataPath + "/_SendMe/Tags/SENDME_tags_0.json";
        playerStatsFile = Application.dataPath + "/_SendMe/Stats/SENDME_stats_0.json";
        if(firstTime)
        {
            roomsPerRun = new List<int>();
            var tagPerformanceDirectory = Directory.CreateDirectory(Application.dataPath + "/_SendMe/Tags/");
            var playerStatsDirectory = Directory.CreateDirectory(Application.dataPath + "/_SendMe/Stats/");
            CheckIfAJsonFileAlreadyExists(DocToWrite.tagPerformance);
            CheckIfAJsonFileAlreadyExists(DocToWrite.PlayerStats);
            File.WriteAllText(tagPerformanceFile, "");    // used to initiate the file
            File.WriteAllText(playerStatsFile, "");    // used to initiate the file
            // ScoreManager.OnUpdateScore += UpdateRunRoomsClearedRealtime;    // called everytime a room is cleared for simplicity of references
            PlayerHealthSystem.instance.OnPlayerDied += RunEnded;
            firstTime = false;
        }
        SceneManager.sceneLoaded += StartEnemyList;
        SceneManager.sceneUnloaded += ClearObserver;
        SceneManager.sceneUnloaded += StopCountingPlayTime;
    }

    internal void AddChallengeRunHistory(PossibleChallengeData bestChallenge)
    {
    }

    private void SubscribeToGameOverConditions(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            // ScoreManager.OnUpdateScore += UpdateRunRoomsClearedRealtime;    // called everytime a room is cleared for simplicity of references
            GameManager.instance.OnGameWon += UpdateRunRoomsClearedOnVictory;
            PlayerHealthSystem.instance.OnPlayerDied += RunEnded;
        }
    }

    private void Start()
    {
        GameManager.instance.SetPlayerAssignedNumber(playerAssignedNumber);
    }

    private void StartEnemyList(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            CreatePrintablePerformanceDataForExistingEnemies();
            runCount++;
        }
    }
    
    private void ClearObserver(Scene loadedScene)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            SceneManager.sceneLoaded -= StartEnemyList;
        }
    }
    
    private void StopCountingPlayTime(Scene unloadedScene)
    {
        if (unloadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            playTime += Time.timeSinceLevelLoad;
            SceneManager.sceneUnloaded -= StopCountingPlayTime;
        }
    }

    private void OnApplicationQuit()
    {
        File.WriteAllText(playerStatsFile, GetPlayerStats());
        File.AppendAllText(tagPerformanceFile, GetPlayerStats());
    }

    private string GetPlayerStats()
    {
        string runHistory = "";
        foreach(int run in roomsPerRun)
        {
            runHistory += run.ToString() + ", ";
        }
        return "pt: " + playTime.ToString() + ", rc: " + runCount.ToString() + ", rh: " + runHistory;
    }

    private IEnumerator PerformanceTest()
    {
        yield return new WaitForSecondsRealtime(1f);
        WritePerformanceForEnemy(EnemyLibrary.instance.GetRandomEnemy().GetEnemyTypeTag(), Random.Range(1, 100));
        StartCoroutine(PerformanceTest());
    }

    private void CreatePrintablePerformanceDataForExistingEnemies()
    {
        foreach(TypeOfEnemy enemy in EnemyLibrary.instance.GetAllPossibleEnemies())
        {
            performanceDataCollection.Add(new PrintablePerformanceData(enemy.GetEnemyTypeTag()));
        }
    }

    public void WritePerformanceForEnemy(string enemyTag, int performance)
    {
        foreach(PrintablePerformanceData performanceData in performanceDataCollection)
        {
            if (string.Equals(performanceData.enemyTag, enemyTag))
            {
                performanceData.performanceValues.Add(performance);
            }
        }
        AddPerformanceToJson();
    }

    private void AddPerformanceToJson()
    {
        string performanceDataForAllEnemies = "";
        foreach (PrintablePerformanceData pd in performanceDataCollection)
        {
            performanceDataForAllEnemies += JsonUtility.ToJson(pd) + ", \n";
        }
        File.WriteAllText(tagPerformanceFile, performanceDataForAllEnemies);
    }
    
    private void CheckIfAJsonFileAlreadyExists(DocToWrite docToWrite)
    {
        switch (docToWrite)
        {
            case (DocToWrite.tagPerformance):
                if(File.Exists(tagPerformanceFile))
                {
                    int i = 1;
                    tagPerformanceFile = Application.dataPath + "/_SendMe/Tags/SENDME_tags_" + i + ".json";
                    while (File.Exists(tagPerformanceFile))
                    {
                        i++;
                        tagPerformanceFile = Application.dataPath + "/_SendMe/Tags/SENDME_tags_" + i + ".json";
                    }
                    playerAssignedNumber = i;
                }
                break;
            case (DocToWrite.PlayerStats):
                if (File.Exists(playerStatsFile))
                {
                    int i = 1;
                    playerStatsFile = Application.dataPath + "/_SendMe/Stats/SENDME_stats_" + i + ".json";
                    while(File.Exists(playerStatsFile))
                    {
                        i++;
                        playerStatsFile = Application.dataPath + "/_SendMe/Stats/SENDME_stats_" + i + ".json";
                    }
                }
                break;
        }
    }
    
    private void UpdateRunRoomsClearedRealtime(int roomsCleared)
    {
        roomClearedSoFarInThisRun = roomsCleared;
    }

    private void RunEnded()
    {
        roomsPerRun.Add(roomClearedSoFarInThisRun);
    }

    public void UpdateRunRoomsClearedOnVictory()
    {
        roomClearedSoFarInThisRun = DungeonManager.instance.GetNumberOfRoomsToBeatDungeon();
        RunEnded();
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
        DontDestroyOnLoad(gameObject);
    }
}
