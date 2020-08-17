using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class SamePlayer : MonoBehaviour
{
    public GameObject _note;
    private bool _playersFolderExist = false;

    private void Awake()
    {
        _note.SetActive(false);
        StartCoroutine(CheckIfThePlayersFolderExists());
    }

    private void Start()
    {
        
    }

    private IEnumerator CheckIfThePlayersFolderExists()
    {
        //Check if the folder exists in the server
        using (UnityWebRequest www = UnityWebRequest.Get("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/" + PlayerPrefs.GetString("playerID") + "/Get_HighScore.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                _playersFolderExist = false;
            }
            else
            {
                _playersFolderExist = true;
            }
        }
        //if the folder doesnt exist or is a fresh player then deactivate btn
        if (!_playersFolderExist || PlayerPrefs.GetString("playerID", "none") == "none")
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
            _note.SetActive(true);
        }
    }
}
