using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;

    public event Action<bool> OnGameIsPaused = delegate { };

    [SerializeField] GameObject menuHolder;

    public static bool gameIsPaused = false;

    private const string pauseMenu = "escape";

    private void Awake()
    {
        MakeThisObjectSingleton();
    }

    private void Start()
    {
        menuHolder.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseMenu))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
            menuHolder.SetActive(!menuHolder.activeSelf);
            // OnGameIsPaused(menuHolder.activeSelf);
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
        JsonWriter.instance.SaveMonsterOrderToFile();
        SceneManager.LoadScene(GameManager.instance.GetVictorySceneNumber());
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
