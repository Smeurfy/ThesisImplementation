using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour 
{        
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GameManager.instance.ResetGame();
        ShowFinalScreen();
    }

    private void ShowFinalScreen()
    {
        SceneManager.LoadScene(2);
    }
}
