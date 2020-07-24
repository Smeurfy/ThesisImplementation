using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewPlayer : MonoBehaviour
{
    public void NewPlayerSelection()
    {
        PlayerPrefs.SetInt("run", 0);
        StartCoroutine(PlayersIDManager.GetNewID());
    }
}
