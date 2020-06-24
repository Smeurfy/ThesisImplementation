using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    private void UndoHealthBonus()
    {
        _score = _scoreBeforeChallenge;
        _currentScore.text = "Score: " + _score;
    }

    // Update is called once per frame
    void Update()
    {

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
