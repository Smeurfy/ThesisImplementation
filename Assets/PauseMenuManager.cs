using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;
    public event Action<bool> OnGameIsPaused = delegate { };
    [SerializeField] GameObject menuHolder;
    public static bool gameIsPaused = false;
    public Text playerID;
    private const string pauseMenu = "escape";
    public GameObject loadingImage;

    private void Awake()
    {
        MakeThisObjectSingleton();
    }

    private void Start()
    {
        menuHolder.SetActive(false);
    }

    private void OnEnable()
    {
        playerID.text = "Your ID is: " + PlayerPrefs.GetString("playerID");
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseMenu))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
            menuHolder.SetActive(!menuHolder.activeSelf);
        }
    }

    void PauseGame()
    {
        if (gameIsPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void QuitGame()
    {
        Instantiate(loadingImage, transform.parent);
        JsonWriter.instance.SaveDataToLogs("GiveUpPauseMenu");
        GameObject.Find("player").SetActive(false);
        gameIsPaused = !gameIsPaused;
        StatsForScoreScreen.CalculateStats();
        HighScore.instance.SaveHighScore();
        Time.timeScale = 1;
        JsonWriter.instance.SaveLogs(true);

    }

    public void ResumeGame()
    {
        menuHolder.SetActive(false);
        gameIsPaused = !gameIsPaused;
        PauseGame();
        print("resume game");
        // OnGameIsPaused(menuHolder.activeSelf);
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
