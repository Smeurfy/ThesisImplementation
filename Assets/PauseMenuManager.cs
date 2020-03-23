using System;
using UnityEngine;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;

    public event Action<bool> OnGameIsPaused = delegate { };

    [SerializeField] GameObject menuHolder;
    [SerializeField] TextMeshProUGUI assignedNumber;

    private const string pauseMenu = "escape";

    private void Awake()
    {
        MakeThisObjectSingleton();
    }

    private void Start()
    {
        menuHolder.SetActive(false);
        assignedNumber.text += GameManager.instance.GetPlayerAssignedNumber().ToString();
        menuHolder.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(pauseMenu))
        {
            menuHolder.SetActive(!menuHolder.activeSelf);
            OnGameIsPaused(menuHolder.activeSelf);
        }
    }

    public void QuitGame()
    {
        print("quit game");
        Application.Quit();
    }

    public void ResumeGame()
    {
        menuHolder.SetActive(false);
        print("resume game");
        OnGameIsPaused(menuHolder.activeSelf);
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
