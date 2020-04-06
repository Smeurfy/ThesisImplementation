using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour 
{
    [SerializeField] private int dungeonIndex;
    [SerializeField] private int creditsIndex;

    public void StartGame()
    {
        SceneManager.LoadScene(dungeonIndex);
    }

    public void Credits()
    {
        //SceneManager.LoadScene(creditsIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
