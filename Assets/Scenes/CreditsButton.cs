using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsButton : MonoBehaviour 
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("main menu");
    }
}
