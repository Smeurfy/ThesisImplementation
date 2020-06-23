using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBonus : MonoBehaviour
{
    public event Action OnHeartFull = delegate { };
    public static HealthBonus instance;
    public Image imgPlaceholder;
    public Animator animHeart;
    private int challengeIndex;
    private RoomManager room;

    private float _healthBonusBeforeChallenge = 0;
    private float _currentHpProgress = 0;
    float fillSpeed = 0.2f;

    private void Awake()
    {
        MakeThisObjectSingleton();
    }
    void Start()
    {
        challengeIndex = DungeonManager.instance.indexChallenge;
        GameObject.Find("player").GetComponent<PlayerHealthSystem>().OnPlayerDied += UndoHealthBonus;
    }

    private void UndoHealthBonus()
    {
        if (_currentHpProgress > _healthBonusBeforeChallenge)
        {
            _currentHpProgress -= 0.25f;
        }
    }
    void Update()
    {
        if (imgPlaceholder.fillAmount < _currentHpProgress)
        {
            animHeart.SetBool("gainBonus", true);
            imgPlaceholder.fillAmount += fillSpeed * Time.deltaTime;
        }
        else
        {
            imgPlaceholder.fillAmount = _currentHpProgress;
            animHeart.SetBool("gainBonus", false);
        }
    }

    public void SubscribeToRoom()
    {
        DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).RoomCleared += UpdateHealthBonus;
        _healthBonusBeforeChallenge = imgPlaceholder.fillAmount;

    }

    private void UpdateHealthBonus()
    {
        _currentHpProgress += 0.25f;
        if (_currentHpProgress == 1)
        {
            imgPlaceholder.fillAmount = 0;
            _currentHpProgress = 0;
            OnHeartFull();
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
