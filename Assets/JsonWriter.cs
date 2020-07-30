using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text;
using System.ComponentModel;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class JsonWriter : MonoBehaviour
{
    public static JsonWriter instance;
    public List<string> _btnClickedOnDeath = new List<string>();
    public List<int> _health = new List<int>();
    public List<int> _bullets = new List<int>();
    public List<float> _shield = new List<float>();
    public List<int> _roomsOfDeath = new List<int>();

    public List<PossibleChallengeData> _skippedChallenges = new List<PossibleChallengeData>();
    public List<PossibleChallengeData> _tryLaterChallenges = new List<PossibleChallengeData>();
    public List<PossibleChallengeData> _tryNowChallenges = new List<PossibleChallengeData>();

    public bool _resetValues;

    public int run;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        MakeThisObjectSingleton();
    }

    private void Start()
    {
        _resetValues = true;
        SceneManager.sceneLoaded += ResetValues;
        run = PlayerPrefs.GetInt("run");
        run++;
        PlayerPrefs.SetInt("run", run);
    }

    private void ResetValues(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 1 && _resetValues)
        {
            _btnClickedOnDeath = new List<string>();
            _health = new List<int>();
            _bullets = new List<int>();
            _shield = new List<float>();
            _roomsOfDeath = new List<int>();
            _skippedChallenges = new List<PossibleChallengeData>();
            _tryLaterChallenges = new List<PossibleChallengeData>();
            _tryNowChallenges = new List<PossibleChallengeData>();
            run = PlayerPrefs.GetInt("run");
            run++;
            PlayerPrefs.SetInt("run", run);
        }
    }

    public void SaveLogs()
    {
        DateTime dateTime = DateTime.Now;

        string path = Application.dataPath + "/Resources/" + "data_" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + "_" + dateTime.Day + "_" + dateTime.Month + "_" + dateTime.Year + ".json";
        FileStream stream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
        {
            for (int i = 0; i < _btnClickedOnDeath.Count; i++)
            {
                writer.WriteLine("Option: " + _btnClickedOnDeath[i]);
                writer.WriteLine("Life: " + _health[i]);
                writer.WriteLine("Bullets: " + _bullets[i]);
                writer.WriteLine("Shield: " + _shield[i]);
                writer.WriteLine("Room: " + _roomsOfDeath[i]);
                writer.WriteLine();
            }
            writer.WriteLine("Rooms cleared: " + StatsForScoreScreen._roomsCleared);
            writer.WriteLine("Score: " + StatsForScoreScreen._score);
            writer.WriteLine("Skips: " + StatsForScoreScreen._skips);
            writer.WriteLine("Time: " + System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2));
            if (_skippedChallenges.Count > 0 || _tryLaterChallenges.Count > 0 || _tryNowChallenges.Count > 0)
            {
                writer.WriteLine("Skipped challenges");
                foreach (var item in _skippedChallenges)
                {
                    writer.WriteLine(item.GetTypeOfEnemies()[0].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[0]] + " " +
                                     item.GetTypeOfEnemies()[1].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[1]]);

                }
                writer.WriteLine("Try later challenges");
                foreach (var item in _tryLaterChallenges)
                {
                    writer.WriteLine(item.GetTypeOfEnemies()[0].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[0]] + " " +
                                     item.GetTypeOfEnemies()[1].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[1]]);

                }
                writer.WriteLine("Try now challenges");
                foreach (var item in _tryNowChallenges)
                {
                    writer.WriteLine(item.GetTypeOfEnemies()[0].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[0]] + " " +
                                     item.GetTypeOfEnemies()[1].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[1]]);

                }
                writer.WriteLine("----------------------------------------");
            }
            else
            {
                writer.WriteLine("Player quit without dying or won the game");
                writer.WriteLine("----------------------------------------");
            }
            writer.WriteLine();
        }
        stream.Close();
        StartCoroutine(PostLogs(path));
    }

    private IEnumerator PostLogs(string path)
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        var fileContent = System.IO.File.ReadAllText(path);
        wwwForm.Add(new MultipartFormFileSection("file", fileContent, null, "data" + PlayerPrefs.GetInt("run")));
        wwwForm.Add(new MultipartFormDataSection("playerID", PlayerPrefs.GetString("playerID")));
        using (UnityWebRequest www = UnityWebRequest.Post("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/LogsManager.php", wwwForm))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                // Debug.Log(www.downloadHandler.text);
                // Debug.Log("success");
            }
        }
        _resetValues = true;
        SceneManager.LoadScene("HighScore");
    }


    private void MakeThisObjectSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
