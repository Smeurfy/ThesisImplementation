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
        AfterDeathOptions.instance.OnTryAgain += SubscribeAgain;
    }

    private void SubscribeAgain()
    {
        ScoreManager.OnUpdateScore += UpdateScoreBoard;
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
        if (this != null)
        {
            int i = 0;
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                if (image.type == Image.Type.Filled)
                {
                    if (image.fillAmount == 0 && scoreImages[i].fillAmount == 1)
                    {
                        image.GetComponent<ParticleSystem>().Play();
                    }
                    image.fillAmount = scoreImages[i].fillAmount;
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


    private void UpdateScoreBoard(int roomsClearedCount)
    {
        int indexToUpdate = --roomsClearedCount;
        if (indexToUpdate == 0)  //cleared first room
        {
            for (int i = 0; i < scoreImages.Count; i++)
            {
                scoreImages[i].fillAmount = 0;
            }
            scoreImages[0].fillAmount = 1;
        }
        else
        {
            for (int i = 0; i <= indexToUpdate; i++)
            {
                scoreImages[i].fillAmount = 1;
            }
        }
        if (roomsClearedCount != GameManager.instance.GetNumberOfRoomsToVictory() - 1)
        {
            FillImages();
        }
        else
        {
            FillImages();
            PlayAllParticles();
        }
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
        GetComponentInParent<RoomManager>().OnPlayerEnteredNewRoom -= FillImages;
        ScoreManager.OnUpdateScore -= UpdateScoreBoard;
        ScoreManager.OnWinAchieved -= UnsubscribeFillImages;
        PlayerHealthSystem.instance.OnPlayerDied -= UnsubscribeFillImages;
    }
}
