using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] private int dungeonIndex;
    [SerializeField] private int creditsIndex;
    public GameObject _loading;

    private void Start()
    {
        _loading.SetActive(false);
    }

    public void StartGame()
    {
        _loading.SetActive(true);
        SceneManager.LoadSceneAsync(dungeonIndex);
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
