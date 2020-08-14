using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;

public class AcknowledgementScreen : MonoBehaviour
{
    public TextMeshProUGUI _text;
    public Button mainMenu;
    public Button exitGame;
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
        if (_text != null)
        {
            _text.text = "Your ID is: " + PlayerPrefs.GetString("playerID");
            StartCoroutine(EnableBtn());
        }
    }

    private IEnumerator EnableBtn()
    {
        yield return new WaitForSeconds(1.0f);
        mainMenu.interactable = true;
        exitGame.interactable = true;
    }
}
