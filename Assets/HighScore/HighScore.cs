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
    public int _scoreBeforeChallenge = 0;
    public Text _currentScore;
    public Text _highScore;

    public int _score = 0;
    private void Awake()
    {
        MakeThisObjectSingleton();
    }
    // Use this for initialization
    void Start()
    {
        GameObject.Find("player").GetComponent<PlayerHealthSystem>().OnPlayerDied += UndoScoreBonus;
        SceneManager.sceneLoaded += GetHighScore;
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
                _highScore.text = "High Score: " + www.downloadHandler.text;
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
            catch(NullReferenceException){
                //bah
            }
        }
    }

    private void UndoScoreBonus()
    {
        _score = _scoreBeforeChallenge;
        _currentScore.text = "Score: " + _score;
    }

    public void SubscribeToRoom(GameObject gObj)
    {
        // gObj.GetComponent<Thesis.Enemy.EnemyHealthSystem>().OnEnemyDie += UpdateScore;
        _scoreBeforeChallenge = _score;
    }

    public void UpdateScore(int value)
    {
        _score += value;
        _currentScore.text = "Score: " + _score;
        Debug.Log("Updated score");
    }

    public void SaveHighScore()
    {
        StartCoroutine(PostHighScore());
    }
    private IEnumerator PostHighScore()
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("currentScore", _score.ToString()));
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
