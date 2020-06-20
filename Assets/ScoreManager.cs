using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static event Action OnWinAchieved;
    public static event Action OnUpdateScore;
    public static event Action OnReachedShieldUnlockRoom;
    ///<summary>
    ///-1 for initialization
    ///0 for skiped challenge
    ///1 for won challenge 
    ///2 for try later challenge
    ///</summary>
    public List<int> _victoryAndLoses = new List<int>();

    [SerializeField] private int numberOfRoomsToWin = 2;
    [SerializeField] private ScoreBoardManager scoreBoard;
    [SerializeField] private int activateShieldOnRoomNumber = 1;

    private int roomsClearedCount = 0;

    private void Start()
    {
        InitializeWinLoseList();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void InitializeWinLoseList()
    {
        for (int i = 0; i < 25; i++)
        {
            _victoryAndLoses.Add(-1);
        }
    }

    public int GetNumberOfRoomsCleared()
    {
        return roomsClearedCount;
    }

    public int GetNumberOfRoomsToBeatDungeon()
    {
        return numberOfRoomsToWin;
    }

    public void UpdateScore(bool value)
    {
        if (value)
        {
            OnUpdateScore();
            if (DungeonManager.instance.DungeonBeaten())
            {
                OnWinAchieved();
            }
        }
    }

    public void UpdateScoreBeforeExitRoom()
    {
        if (DungeonManager.instance.playersRoom != -1)
        {
            _victoryAndLoses[DungeonManager.instance.indexChallenge] = 1;
        }
        if (DefeatedRoomsToUnlockShield())
        {
            OnReachedShieldUnlockRoom();
        }
        OnUpdateScore();
    }

    public void UndoScore()
    {
        if (DungeonManager.instance.playersRoom != -1 && _victoryAndLoses[DungeonManager.instance.indexChallenge] != 2)
            _victoryAndLoses[DungeonManager.instance.indexChallenge] = -1;
        OnUpdateScore();
    }

    private bool DefeatedRoomsToUnlockShield()
    {
        int wins = 0;
        foreach (var item in _victoryAndLoses)
        {
            if (item == 1)
                wins++;
        }
        if (wins == activateShieldOnRoomNumber)
            return true;
        return false;
    }

    private void SceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            roomsClearedCount = 0;
            _victoryAndLoses = new List<int>();
            InitializeWinLoseList();
            scoreBoard = GameObject.FindGameObjectWithTag("FirstRoom").GetComponentInChildren<ScoreBoardManager>();
        }
    }
}
