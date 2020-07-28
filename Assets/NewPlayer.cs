using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewPlayer : MonoBehaviour
{
    public GameObject _loading;

    private void Start()
    {
        _loading.SetActive(false);
    }

    public void NewPlayerSelection()
    {
        _loading.SetActive(true);
        PlayerPrefs.SetInt("run", 0);
        StartCoroutine(PlayersIDManager.GetNewID());
    }
}
