using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsForScoreScreen : MonoBehaviour
{
    public static int _roomsCleared = 0;
    public static int _monstersDefeated = 0;
    public static int _skips = 0;
    public static int _time = 0;
    public static int _score = 0;

    private void Start()
    {
        _roomsCleared = 0;
        _monstersDefeated = 0;
        _skips = 0;
        _time = 0;
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
    }
}
