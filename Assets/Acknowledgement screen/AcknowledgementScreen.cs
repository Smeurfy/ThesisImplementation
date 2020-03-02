using UnityEngine;
using UnityEngine.SceneManagement;

public class AcknowledgementScreen : MonoBehaviour 
{
    public void CloseGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("main menu");
    }
}
