using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AcknowledgementScreen : MonoBehaviour
{
    public TextMeshProUGUI _text;
    public void CloseGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("main menu");
    }

    private void OnEnable()
    {
        _text.text = "Your ID is: " + PlayerPrefs.GetString("playerID");
    }
}
