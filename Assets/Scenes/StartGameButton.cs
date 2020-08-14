using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public GameObject _loading;

    private void Start()
    {
        if(_loading != null)
            _loading.SetActive(false);
    }

    public void StartGame()
    {
        _loading.SetActive(true);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Credits()
    {
        SceneManager.LoadScene("credits");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
