using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static event Action OnWinAchieved;
    public static event Action<int> OnUpdateScore;
    public static event Action OnReachedShieldUnlockRoom;

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

    public void UpdateScore()
    {
        roomsClearedCount++;
        Debug.Log("Entrei no update");
        OnUpdateScore(roomsClearedCount);
        if (DungeonManager.instance.DungeonBeaten())
        {
            OnWinAchieved();
        }
        if (roomsClearedCount == activateShieldOnRoomNumber)
        {
            OnReachedShieldUnlockRoom();
        }
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
