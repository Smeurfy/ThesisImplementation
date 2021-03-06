﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AfterDeathOptions : MonoBehaviour
{
    public static AfterDeathOptions instance;

    public GameObject afterDeathMenu;
    public GameObject health;
    public GameObject bullets;
    public GameObject shield;
    public GameObject loadingImage;

    public Button tryAgainLater;

    public event Action OnTryAgainNow = delegate { };
    public event Action OnTryAgainLater = delegate { };
    public event Action OnRestartSameRun = delegate { };
    public event Action OnRestartNewRun = delegate { };
    public event Action OnSkip = delegate { };

    private void Awake()
    {
        MakeThisObjectSingleton();
    }

    // Use this for initialization
    void Start()
    {
        afterDeathMenu.SetActive(false);
    }

    public void TryAgainNow()
    {
        if (OnTryAgainNow != null)
        {
            JsonWriter.instance._btnClickedOnDeath.Add("TryNow");
            OnTryAgainNow();
        }
    }
    public void TryAgainLater()
    {
        if (OnTryAgainNow != null)
        {
            JsonWriter.instance._btnClickedOnDeath.Add("TryLater");
            OnTryAgainLater();
        }
    }

    public void RestartSameRun()
    {
        Instantiate(loadingImage, transform.parent);
        JsonWriter.instance._btnClickedOnDeath.Add("SameRun");
        JsonWriter.instance._roomChallenge.Add(DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).challengeOfThisRoom);
        StatsForScoreScreen.CalculateStats();
        JsonWriter.instance.SaveLogs(false);
        OnRestartSameRun();
    }

    public void RestartNewRun()
    {
        Instantiate(loadingImage, transform.parent);
        JsonWriter.instance._btnClickedOnDeath.Add("NewRun");
        JsonWriter.instance._roomChallenge.Add(DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).challengeOfThisRoom);
        StatsForScoreScreen.CalculateStats();
        JsonWriter.instance.SaveLogs(false);
        OnRestartNewRun();
    }

    public void Skip()
    {
        JsonWriter.instance._btnClickedOnDeath.Add("Skipped");
        OnSkip();
    }

    public void GiveUp()
    {
        Instantiate(loadingImage, transform.parent);
        JsonWriter.instance._btnClickedOnDeath.Add("GiveUp");
        JsonWriter.instance._roomChallenge.Add(DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).challengeOfThisRoom);
        StatsForScoreScreen.CalculateStats();
        HighScore.instance.SaveHighScore();
        JsonWriter.instance.SaveLogs(true);
    }

    public void UpdateBulletUI(int bullet)
    {
        bullets.GetComponentInChildren<Text>().text = bullet + " Bullets";
    }
    public void UpdateShieldUI(float shield)
    {
        this.shield.GetComponentInChildren<Text>().text = Math.Round(shield * 100, 0) + "% Shield";
        this.shield.GetComponent<Image>().fillAmount = shield;
    }

    public void UpdateHealthUI(int hp)
    {
        health.GetComponentInChildren<Text>().text = hp + " Health";
    }

    public void OnEnable()
    {
        try
        {
            var nextIndex = DungeonManager.instance.indexChallenge + 2;
            if (nextIndex > 24)
            {
                tryAgainLater.interactable = false;
            }
            else
            {
                tryAgainLater.interactable = true;
            }
        }
        catch (NullReferenceException)
        {
            Debug.Log("null reference");
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
