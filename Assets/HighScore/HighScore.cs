using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class HighScore : MonoBehaviour
{
    public static HighScore instance;
    public GameObject _currentScore;
    public GameObject _highScore;

    public int _scoreBeforeChallenge = 0;
    public int _score = 0;
    public GameObject _player;
    private void Awake()
    {
        MakeThisObjectSingleton();
    }
    // Use this for initialization
    void Start()
    {
        _player = FindObjectOfType<PlayerHealthSystem>().gameObject;
        _player.GetComponent<PlayerHealthSystem>().OnPlayerDied += UndoScoreBonus;
        SceneManager.sceneLoaded += GetHighScore;
        SceneManager.sceneUnloaded += RemoveEvents;
    }

    private void RemoveEvents(Scene arg0)
    {
        if (arg0.buildIndex == GameManager.instance.GetMainGameSceneNumber())
            _player.GetComponent<PlayerHealthSystem>().OnPlayerDied -= UndoScoreBonus;
    }

    public IEnumerator GetHighScoreServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/" + PlayerPrefs.GetString("playerID") + "/Get_HighScore.php"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                StartCoroutine(GetHighScoreServer());
            }
            else
            {
                // Show results as text
                _highScore.GetComponent<Text>().text = "High Score: " + www.downloadHandler.text;
            }
        }
    }

    private void GetHighScore(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            SceneManager.sceneLoaded -= GetHighScore;
            try
            {
                StartCoroutine(GetHighScoreServer());
            }
            catch (MissingReferenceException)
            {
                //bah
            }
            catch (NullReferenceException)
            {
                //bah
            }
        }
    }

    private void UndoScoreBonus()
    {
        _score = _scoreBeforeChallenge;
        try
        {
            // _currentScore.text = "Score: " + _score;
            _currentScore.GetComponent<Text>().text = "Score: " + _score;
        }
        catch (MissingReferenceException)
        {
            //bah
        }
        catch (NullReferenceException)
        {
            //bah
        }
    }

    public void SubscribeToRoom()
    {
        _scoreBeforeChallenge = _score;
    }

    public void UpdateScore(int value)
    {
        _score += value;
        _currentScore.GetComponent<Text>().text = "Score: " + _score;
    }

    public void SaveHighScore()
    {
        StartCoroutine(PostHighScore());
    }
    private IEnumerator PostHighScore()
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("currentScore", "" + (StatsForScoreScreen._score + System.Math.Round((StatsForScoreScreen._score / System.Math.Round(StatsForScoreScreen._time.TotalSeconds, 2)), 2))));
        wwwForm.Add(new MultipartFormDataSection("playerID", PlayerPrefs.GetString("playerID")));

        UnityWebRequest www = UnityWebRequest.Post("http://web.tecnico.ulisboa.pt/~ist424747/HolidayKnight/Post_HighScore.php", wwwForm);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            // Debug.Log(www.downloadHandler.text);
        }
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
    }
}
