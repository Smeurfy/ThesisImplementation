using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatsForScoreScreen : MonoBehaviour
{
    public static int _roomsCleared = 0;
    public static int _monstersDefeated = 0;
    public static int _skips = 0;
    public static int _score = 0;
    public static TimeSpan _time;
    public static DateTime startTime;

    private void Start()
    {
        _roomsCleared = 0;
        _monstersDefeated = 0;
        _skips = 0;
        startTime = DateTime.Now;
        _score = 0;
    }

    public static void CalculateStats()
    {
        List<int> victoryAndLoses = FindObjectOfType<ScoreManager>()._victoryAndLoses;
        foreach (var item in victoryAndLoses)
        {
            if (item == 1)
            {
                _roomsCleared++;
            }
            if (item == 0)
            {
                _skips++;
            }
        }
        _monstersDefeated = _roomsCleared * 2;
        _score = HighScore.instance._score;
        var timeNow = DateTime.Now;
        _time = timeNow - startTime;
    }
}
