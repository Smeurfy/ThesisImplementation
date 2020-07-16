using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaterChallengePopUp : MonoBehaviour
{
    private List<int> victoryAndLoses = new List<int>();
    public GameObject popUp;

    // Use this for initialization
    void Start()
    {
        popUp.SetActive(false);
    }

    private IEnumerator EnablePopUp()
    {

        yield return new WaitForSecondsRealtime(2);
        popUp.SetActive(false);
    }
    public void ShowPopUp()
    {
        popUp.SetActive(true);
        StartCoroutine(EnablePopUp());
    }
}
