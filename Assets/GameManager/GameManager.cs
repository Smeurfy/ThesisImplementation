﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public event Action OnGameWon = delegate { };

    [SerializeField] private int gameSceneNumber;
    [SerializeField] private int victorySceneNumber;
    [SerializeField] private float secondsToWaitBeforeReset;
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip victorySound;

    private int playerAssignedNumber = -5;
    private DungeonManager dungeonManager;
    private Vector2 playerRespawnPosition = Vector2.zero;
    private bool reseting = false;
    private Animator fadeToBlack;
    private AudioSource soundSource;

    #region getters
    public Transform GetPlayerReference() { return player; }
    public int GetMainGameSceneNumber() { return gameSceneNumber; }
    public int GetVictorySceneNumber() { return victorySceneNumber; }
    public Vector2 GetRespawnPosition() { return playerRespawnPosition; }
    public int GetPlayerAssignedNumber() { return playerAssignedNumber; }
    public int GetNumberOfRoomsToVictory() { return GetComponent<ScoreManager>().GetNumberOfRoomsToBeatDungeon(); }
    #endregion

    private void Awake()
    {
        MakeThisObjectSingleton();
        player = FindObjectOfType<PlayerHealthSystem>().transform;
        dungeonManager = GetComponent<DungeonManager>();
        fadeToBlack = Camera.main.GetComponentInChildren<Animator>();
        ScoreManager.OnWinAchieved += WinGame;
        SceneManager.sceneUnloaded += Dereference;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += EnablePlayerAndGetWinFadeReference;
        AfterDeathOptions.instance.OnRestartSameRun += ResetGame;
        AfterDeathOptions.instance.OnRestartNewRun += ResetGame;
    }

    private void Dereference(Scene unloadedScene)
    {
        if (unloadedScene.buildIndex == instance.gameSceneNumber)
        {
            ScoreManager.OnWinAchieved -= WinGame;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerHealthSystem>().transform;
        }
    }

    private void WinGame()
    {
        StatsForScoreScreen.CalculateStats();
        HighScore.instance.SaveHighScore();
        JsonWriter.instance.SaveLogs(true);
        if (!soundSource)
        {
            soundSource = instance.GetComponent<AudioSource>();
        }
        soundSource.PlayOneShot(victorySound);
        OnGameWon();
        fadeToBlack.enabled = true;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player)
        {
            ResetAndDisablePlayer();
        }
    }

    private void ResetAndDisablePlayer()
    {
        player.transform.position = playerRespawnPosition;
        player.GetComponent<PlayerHealthSystem>().EnablePlayerControls();
        player.gameObject.SetActive(false);
    }

    internal void ResetGame()
    {
        StatsForScoreScreen.CalculateStats();
        HighScore.instance.SaveHighScore();
        Debug.Log("resetGame");
        if (!reseting)
        {
            reseting = true;
            if (this == null)
            {
                instance.StartCoroutine(WaitAndResetGame());
            }
            else { StartCoroutine(WaitAndResetGame()); }
        }

    }

    private IEnumerator WaitAndResetGame()
    {
        Debug.Log("waitAndReset");
        yield return new WaitForSecondsRealtime(secondsToWaitBeforeReset);
        SceneManager.LoadScene(gameSceneNumber);
        player.GetComponent<PlayerHealthSystem>().EnablePlayerControls();
        AfterDeathOptions.instance.OnRestartSameRun -= ResetGame;
        AfterDeathOptions.instance.OnRestartNewRun -= ResetGame;
        reseting = false;
    }

    private void EnablePlayerAndGetWinFadeReference(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == instance.gameSceneNumber)
        {
            if (player)
                player.gameObject.SetActive(true);
            fadeToBlack = Camera.main.GetComponentInChildren<Animator>();
            AfterDeathOptions.instance.OnRestartSameRun += ResetGame;
            AfterDeathOptions.instance.OnRestartNewRun += ResetGame;
        }
    }

    public void SetPlayerAssignedNumber(int numberToAssign)
    {
        playerAssignedNumber = numberToAssign;
    }

    private void MakeThisObjectSingleton()
    {
        DontDestroyOnLoad(gameObject);
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
