using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static event Action OnWinAchieved;
    public static event Action<int> OnUpdateScore;
    public static event Action OnReachedShieldUnlockRoom;
    ///<summary>
    ///0 for skiped challenge and 1 for won challenge
    ///</summary>
    public List<int> _victoryAndLoses = new List<int>();

    [SerializeField] private int numberOfRoomsToWin = 2;
    [SerializeField] private ScoreBoardManager scoreBoard;
    [SerializeField] private int activateShieldOnRoomNumber = 1;

    private int roomsClearedCount = 0;

    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
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
            roomsClearedCount++;
            OnUpdateScore(roomsClearedCount);
            if (DungeonManager.instance.DungeonBeaten())
            {
                OnWinAchieved();
            }
            if (DefeatedRoomsToUnlockShield())
            {
                OnReachedShieldUnlockRoom();
            }
        }
    }

    private bool DefeatedRoomsToUnlockShield()
    {
        int wins = 0;
        foreach (var item in _victoryAndLoses)
        {
            if(item == 1)
                wins++;
        }
        if(wins == activateShieldOnRoomNumber)
            return true;
        return false;
    }

    private void SceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            roomsClearedCount = 0;
            scoreBoard = GameObject.FindGameObjectWithTag("FirstRoom").GetComponentInChildren<ScoreBoardManager>();
        }
    }
}
