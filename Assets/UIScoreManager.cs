using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class UIScoreManager : MonoBehaviour
{
    public Text roomsCleared;
    public Text monstersDeafeated;
    public Text skips;
    public Text time;
    public Text score;
    public Text highScore;

    private void Awake()
    {
        StartCoroutine(GetHighScoreServer());
        PopulateWithStats();
    }
	
    private void PopulateWithStats()
    {
        roomsCleared.text = "Rooms Cleared: " + StatsForScoreScreen._roomsCleared.ToString();
        monstersDeafeated.text = "Monsters Defeated: " + StatsForScoreScreen._monstersDefeated.ToString();
        skips.text = "Skips: " + StatsForScoreScreen._skips.ToString();
        time.text = "Time: " +  System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2).ToString();
        score.text = "Score: " + StatsForScoreScreen._score.ToString();
    }

    public IEnumerator GetHighScoreServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/" + PlayerPrefs.GetString("playerID") + "/Get_HighScore.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                highScore.text = "High Score: " + www.downloadHandler.text;
            }
        }
    }
}
