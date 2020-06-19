using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour
{
    private static List<Image> scoreImages;
    private static bool firstTime = true;
    private ParticleSystem ps;
    private ScoreManager _scoreManager;

    private void Start()
    {
        _scoreManager = GameManager.instance.GetComponentInChildren<ScoreManager>();
        if (firstTime)
        {
            scoreImages = new List<Image>();
            GetAllScoreImages();
            SceneManager.sceneLoaded += GetImages;
            firstTime = false;
        }
        else
        {
            FillImages();
        }
        PlayerHealthSystem.instance.OnPlayerDied += UnsubscribeFillImages;
        ScoreManager.OnWinAchieved += UnsubscribeFillImages;
        ScoreManager.OnUpdateScore += UpdateScoreBoard;
        DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).GetComponentInChildren<DoorManager>().OnPlayerEnteredRoom += FillImages;
        AfterDeathOptions.instance.OnSkip += FillImages;
    }

    private void GetImages(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            firstTime = true;
            GetAllScoreImages();
        }
    }

    private void FillImages()
    {
        if (this)
        {
            int i = 0;
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                if (image.type == Image.Type.Filled)
                {
                    if (image.fillAmount == 1 && image.color == Color.green && i == DungeonManager.instance.indexChallenge && image.color != scoreImages[i].color)
                    {
                        image.fillAmount = 0;
                    }
                    if (image.fillAmount == 0 && scoreImages[i].fillAmount == 1)
                    {
                        ParticleSystem ps = image.GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule ma = ps.main;
                        ma.startColor = scoreImages[i].color;
                        image.GetComponent<ParticleSystem>().Play();
                    }
                    image.fillAmount = scoreImages[i].fillAmount;
                    image.color = scoreImages[i].color;
                    if (i == DungeonManager.instance.indexChallenge)
                        image.transform.parent.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                    else
                        image.transform.parent.transform.localScale = new Vector3(1f, 1f, 1f);
                    i++;
                }
            }
        }
    }

    private void PlayAllParticles()
    {
        if (this != null)
        {
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                if (image.type == Image.Type.Filled)
                {
                    image.GetComponent<ParticleSystem>().Play();
                }
            }
        }
    }


    private void UpdateScoreBoard()
    {
        // int indexToUpdate = --roomsClearedCount;
        // if (indexToUpdate == 0)  //cleared first room
        // {
        //     for (int i = 0; i < scoreImages.Count; i++)
        //     {
        //         scoreImages[i].fillAmount = 0;
        //     }
        //     if (_scoreManager._victoryAndLoses[0] == 0)
        //         scoreImages[0].color = Color.red;
        //     if (_scoreManager._victoryAndLoses[0] == 1)
        //         scoreImages[0].color = Color.yellow;
        // }
        // else
        // {
        for (int i = 0; i < _scoreManager._victoryAndLoses.Count; i++)
        {
            if (_scoreManager._victoryAndLoses[i] == -1)
                scoreImages[i].fillAmount = 0;
            if (_scoreManager._victoryAndLoses[i] != -1)
                scoreImages[i].fillAmount = 1;
            if (_scoreManager._victoryAndLoses[i] == 0)
                scoreImages[i].color = Color.red;
            if (_scoreManager._victoryAndLoses[i] == 1)
                scoreImages[i].color = Color.yellow;
            if (_scoreManager._victoryAndLoses[i] == 2)
                scoreImages[i].color = Color.green;
        }
        // }
        // if (roomsClearedCount != GameManager.instance.GetNumberOfRoomsToVictory() - 1)
        // {
        //     FillImages();
        // }
        // else
        // {
        FillImages();
        // PlayAllParticles();
        // }
    }

    private void GetAllScoreImages()
    {
        if (this != null)
        {
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                if (image.type == Image.Type.Filled)
                {
                    image.fillAmount = 0;
                    scoreImages.Add(image);
                }
            }
        }
    }

    private void UnsubscribeFillImages()
    {
        // GetComponentInParent<RoomManager>().OnPlayerEnteredNewRoom -= FillImages;
        // ScoreManager.OnUpdateScore -= UpdateScoreBoard;
        // ScoreManager.OnWinAchieved -= UnsubscribeFillImages;
        // PlayerHealthSystem.instance.OnPlayerDied -= UnsubscribeFillImages;
    }
}
