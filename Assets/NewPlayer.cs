using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayer : MonoBehaviour
{
    public void NewPlayerSelection()
    {
        StartCoroutine(PlayersIDManager.GetNewID());
    }
}
