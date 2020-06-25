using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AfterDeathOptions : MonoBehaviour
{
    public static AfterDeathOptions instance;

    [SerializeField]
    public GameObject afterDeathMenu;
    public GameObject health;
    public GameObject bullets;

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
        GameObject.Find("player").GetComponent<PlayerHealthSystem>().OnPlayerDied += CheckTryAgainLaterAvailability;
    }

    public void TryAgainNow()
    {
        if (OnTryAgainNow != null)
        {
            Debug.Log("try again");
            OnTryAgainNow();
        }

    }
    public void TryAgainLater()
    {
        if (OnTryAgainNow != null)
        {
            Debug.Log("try again later");
            OnTryAgainLater();
        }
    }

    public void RestartSameRun()
    {
        Debug.Log("restart same run");
        OnRestartSameRun();
    }

    public void RestartNewRun()
    {
        Debug.Log("restart new run");
        OnRestartNewRun();
    }

    public void Skip()
    {
        Debug.Log("skip");
        OnSkip();
    }

    public void UpdateBulletUI(int bullet)
    {
        bullets.GetComponentInChildren<Text>().text = bullet + " Bullets";
    }

    public void UpdateHealthUI(int hp)
    {
        health.GetComponentInChildren<Text>().text = hp + " Health";
    }

    public void CheckTryAgainLaterAvailability()
    {
        var nextIndex = DungeonManager.instance.indexChallenge + 2;
        if (nextIndex > 24)
        {
            tryAgainLater.interactable = false;
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
