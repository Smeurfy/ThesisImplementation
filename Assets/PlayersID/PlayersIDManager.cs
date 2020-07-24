using System.Collections;
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
        using (UnityWebRequest www = UnityWebRequest.Get("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/IDManager.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                string id = www.downloadHandler.text;
                PlayerPrefs.SetString("playerID", id);
            }
        }
        SceneManager.LoadScene("implemented model");

    }
}
