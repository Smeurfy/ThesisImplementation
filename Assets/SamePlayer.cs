using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class SamePlayer : MonoBehaviour
{
    private bool _playersFolderExist = false;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(CheckIfThePlayersFolderExists());
    }

    private IEnumerator CheckIfThePlayersFolderExists()
    {
		Debug.Log(PlayerPrefs.GetString("playerID"));
        //Check if the folder exists in the server
        using (UnityWebRequest www = UnityWebRequest.Get("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/" + PlayerPrefs.GetString("playerID") + "/Get_HighScore.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("false");
                _playersFolderExist = false;
            }
            else
            {
                Debug.Log("true");
                _playersFolderExist = true;
            }
        }
		//if the folder doesnt exist or fresh player deactivate btn
        if (!_playersFolderExist || PlayerPrefs.GetString("playerID", "none") == "none")
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
        }
    }
}
