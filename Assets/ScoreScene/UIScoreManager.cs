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

    int skipsTotal = 0;

    private void Awake()
    {
        StartCoroutine(GetHighScoreServer());
        PopulateWithStats();
    }

    private void PopulateWithStats()
    {
        roomsCleared.transform.GetComponentsInChildren<Text>()[1].text = "+" + StatsForScoreScreen._score.ToString();
        // skips.transform.GetComponentsInChildren<Text>()[1].text = CalculateSkipPoints() + " Max Score";
        time.transform.GetComponentsInChildren<Text>()[1].text = "+" + System.Math.Round((StatsForScoreScreen._score / System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2)), 2);
        score.transform.GetComponentsInChildren<Text>()[1].text = "" + (StatsForScoreScreen._score + System.Math.Round((StatsForScoreScreen._score / System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2)), 2));

        roomsCleared.text = "Rooms Cleared: " + StatsForScoreScreen._roomsCleared.ToString();
        time.text = "Time: " + System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2).ToString();
        // monstersDeafeated.text = "Monsters Defeated: " + StatsForScoreScreen._monstersDefeated.ToString();
        CalculateSkipPoints();
        skips.text = "Your score is " + (StatsForScoreScreen._score + System.Math.Round((StatsForScoreScreen._score / System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2)), 2)) + "/" + ((StatsForScoreScreen._score + System.Math.Round((StatsForScoreScreen._score / System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2)), 2)) + skipsTotal) + " because you skipped " + StatsForScoreScreen._skips + " rooms";
        
    }

    private string CalculateSkipPoints()
    {
        int total = 0;
        foreach (var challenge in JsonWriter.instance._skippedChallenges)
        {
            foreach (var item in challenge.GetTypeOfEnemies())
            {
                int tier = challenge._enemyTiers[item];
                if (tier == 0 || tier == 1)
                {
                    total += 10;
                }
                else if (tier == 2 || tier == 3)
                {
                    total += 20;
                }
                else
                {
                    total += 30;
                }
            }
        }
        skipsTotal = total == 0 ? 0 : total;
        return total == 0 ? "-0" : ("-" + total);
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
                highScore.transform.GetComponentsInChildren<Text>()[1].text = www.downloadHandler.text;
            }
        }
    }
}
