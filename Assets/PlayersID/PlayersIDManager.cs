﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayersIDManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(HighScore.instance.GetHighScoreServer());
    }

    public static IEnumerator GetNewID()
    {
        var receivedID = false;
        using (UnityWebRequest www = UnityWebRequest.Get("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/IDManager.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                receivedID = false;
                GetNewID();
            }
            else
            {
                // Show results as text
                // Debug.Log(www.downloadHandler.text);
                string id = www.downloadHandler.text;
                PlayerPrefs.SetString("playerID", id);
                receivedID = true;
            }
        }
        if(receivedID)
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
