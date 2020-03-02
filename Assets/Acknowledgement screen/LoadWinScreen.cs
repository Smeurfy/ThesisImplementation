using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWinScreen : MonoBehaviour 
{
    public void LoadWinScreenScene()
    {
        SceneManager.LoadScene(GameManager.instance.GetVictorySceneNumber());
    }
}
