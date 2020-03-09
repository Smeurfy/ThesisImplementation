using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;
    public event Action OnGameOver = delegate { }; 
    public event Action OnGameWon = delegate { }; 

    [SerializeField] private int gameSceneNumber;
    [SerializeField] private int victorySceneNumber;
    [SerializeField] private float secondsToWaitBeforeReset;
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip victorySound;
    //[SerializeField] private bool usingModel = false;

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
    //public bool IsUsingModel() { return usingModel; }
    public int GetPlayerAssignedNumber() { return playerAssignedNumber; }
    public int GetNumberOfRoomsToVictory() { return GetComponent<ScoreManager>().GetNumberOfRoomsToBeatDungeon(); }
    #endregion

    private void Awake()
    {
        MakeThisObjectSingleton();
        player = FindObjectOfType<PlayerHealthSystem>().transform;
        FindObjectOfType<PlayerHealthSystem>().OnPlayerDied += ResetGame;
        dungeonManager = GetComponent<DungeonManager>();
        ScoreManager.OnWinAchieved += WinGame;
        SceneManager.sceneLoaded += EnablePlayerAndGetWinFadeReference;
        SceneManager.sceneUnloaded += Dereference;
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
        if(player == null)
        {
            player = FindObjectOfType<PlayerHealthSystem>().transform;
        }
    }

    private void WinGame()
    {
        if(!soundSource)
        {
            soundSource = instance.GetComponent<AudioSource>();
        }
        soundSource.PlayOneShot(victorySound);
        OnGameWon();
        JsonWriter.instance.UpdateRunRoomsClearedOnVictory();
        fadeToBlack.enabled = true;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(player)
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
        if(!reseting)
        {
            FindObjectOfType<PlayerHealthSystem>().OnPlayerDied -= ResetGame;
            reseting = true;
            if(this == null)
            {
                instance.StartCoroutine(WaitAndResetGame());
            }
            else { StartCoroutine(WaitAndResetGame()); }
        }
    }
    
    private IEnumerator WaitAndResetGame()
    {
        OnGameOver();
        yield return new WaitForSecondsRealtime(secondsToWaitBeforeReset);
        SceneManager.LoadScene(gameSceneNumber);
        player.GetComponent<PlayerHealthSystem>().EnablePlayerControls();
        reseting = false;
    }

    private void EnablePlayerAndGetWinFadeReference(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == instance.gameSceneNumber)
        {
            if (player)
                player.gameObject.SetActive(true);
            fadeToBlack = Camera.main.GetComponentInChildren<Animator>();
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
