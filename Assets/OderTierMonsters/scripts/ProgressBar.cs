using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public ParticleSystem particleSys;
    float fillSpeed = 0.2f;
    float targetProgress = 0;
    private float _progressBeforeChallenge = 0;

    private void Start()
    {
        GameObject.Find("player").GetComponent<PlayerHealthSystem>().OnPlayerDied += UndoProgress;
    }

    private void UndoProgress()
    {
        if(targetProgress > _progressBeforeChallenge){
            targetProgress = _progressBeforeChallenge;
            slider.value = _progressBeforeChallenge;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < targetProgress)
        {
            slider.value += fillSpeed * Time.deltaTime;
            if (!particleSys.isPlaying)
            {
                particleSys.Play();
            }
        }
        else
        {
            particleSys.Stop();
            slider.value = (float)Math.Round(slider.value, 1);
        }
        if (slider.value >= 1)
            GameObject.Find("ProgressBarUI").SetActive(false);
    }

    public void IncreaseProgress(float newProgress)
    {
        targetProgress = (float)Math.Round(slider.value, 1) + newProgress;
    }

    public void SubscribeToRoom()
    {
        DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).RoomCleared += IncreaseProgress;
        _progressBeforeChallenge = slider.value;
    }
    public void IncreaseProgress()
    {
        targetProgress = (float)Math.Round(slider.value, 1) + (1f / GameManager.instance.GetComponent<ScoreManager>().activateShieldOnRoomNumber);
    }
}
