using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersIDManager : MonoBehaviour
{
    public static string _playerID = "";

    // Use this for initialization
    void Start()
    {
        // if (_playerID == "")
        //     StartCoroutine(GetID());
        // else
        StartCoroutine(HighScore.instance.GetHighScoreServer());

    }

    // public IEnumerator GetID()
    // {
    //     using (UnityWebRequest www = UnityWebRequest.Get("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/IDManager.php"))
    //     {
    //         yield return www.SendWebRequest();

    //         if (www.isNetworkError || www.isHttpError)
    //         {
    //             Debug.Log(www.error);
    //         }
    //         else
    //         {
    //             // Show results as text
    //             Debug.Log(www.downloadHandler.text);
    //             _playerID = www.downloadHandler.text;
    //         }
    //     }
    //     StartCoroutine(HighScore.instance.GetHighScoreServer());
    // }

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
        
    }
}
