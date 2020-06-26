using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        GameObject.Find("player").GetComponent<PlayerHealthSystem>().OnPlayerDied += UndoHealthBonus;
        _highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
        SceneManager.sceneLoaded += GetHighScore;
    }

    private void GetHighScore(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= GetHighScore;
        _highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
    }

    private void UndoHealthBonus()
    {
        _score = _scoreBeforeChallenge;
        _currentScore.text = "Score: " + _score;
    }

    public void SubscribeToRoom(GameObject gObj)
    {
        gObj.GetComponent<Thesis.Enemy.EnemyHealthSystem>().OnEnemyDie += UpdateScore;
        _scoreBeforeChallenge = _score;
    }

    private void UpdateScore()
    {
        _score += 10;
        _currentScore.text = "Score: " + _score;
    }

    public void SaveHighScore()
    {
        if (_score > PlayerPrefs.GetInt("HighScore"))
            PlayerPrefs.SetInt("HighScore", _score);
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
