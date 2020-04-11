using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUILeft : MonoBehaviour
{
    public static DebugUILeft instance;
    RoomManager roomManager;
    GameObject challengeBeforeSkip;
    void Start()
    {
        MakeThisObjectSingleton();
        roomManager = DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom);
        roomManager.GetComponentInChildren<DoorManager>().OnPlayerEnteredRoom += UpdateUI;
    }

    private void UpdateUI()
    {
        roomManager = DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom);
        var thisChallenge = GameObject.Find("ThisChallenge").GetComponentsInChildren<Image>();
        for (int i = 0; i < thisChallenge.Length; i++)
        {
            thisChallenge[i].sprite = EnemyLibrary.instance.GetEnemyTypePrefab(roomManager.challengeOfThisRoom.GetTypeOfEnemies()[i]).GetComponent<SpriteRenderer>().sprite;
            thisChallenge[i].preserveAspect = true;
            thisChallenge[i].GetComponentInChildren<Text>().text = "Tier: " + DungeonManager.instance.tierOfEnemies[roomManager.challengeOfThisRoom.GetTypeOfEnemies()[i]];
        }
        challengeBeforeSkip = GameObject.Find("ThisChallenge");
        if (DungeonManager.instance.playersRoom != -1 && DungeonManager.instance.playersRoom != 0)
        {
            var previousChallenge = GameObject.Find("PreviousChallenge").GetComponentsInChildren<Image>();
            var previousRoomPlayer = DungeonManager.instance.playersRoom - 1;
            var previousRoomManager = DungeonManager.instance.GetRoomManagerByRoomID(previousRoomPlayer);
            for (int i = 0; i < previousChallenge.Length; i++)
            {
                previousChallenge[i].sprite = EnemyLibrary.instance.GetEnemyTypePrefab(previousRoomManager.challengeOfThisRoom.GetTypeOfEnemies()[i]).GetComponent<SpriteRenderer>().sprite;
                previousChallenge[i].preserveAspect = true;
                previousChallenge[i].GetComponentInChildren<Text>().text = "Tier: " + DungeonManager.instance.tierOfEnemies[previousRoomManager.challengeOfThisRoom.GetTypeOfEnemies()[i]];
            }
        }
        roomManager.GetComponentInChildren<DoorManager>().OnPlayerEnteredRoom += UpdateUI;
    }

    private void DoSkipStuff()
    {
        var previousChallenge = GameObject.Find("PreviousChallenge").GetComponentsInChildren<Image>();
        for (int i = 0; i < previousChallenge.Length; i++)
        {
            previousChallenge[i].sprite = challengeBeforeSkip.GetComponentsInChildren<Image>()[i].sprite;
            previousChallenge[i].preserveAspect = true;
            previousChallenge[i].GetComponentInChildren<Text>().text = challengeBeforeSkip.GetComponentsInChildren<Image>()[i].GetComponentInChildren<Text>().text;
        }

    }

    public void UpdateThisChallenge()
    {
        DoSkipStuff();

        var thisChallenge = GameObject.Find("ThisChallenge").GetComponentsInChildren<Image>();
        for (int i = 0; i < thisChallenge.Length; i++)
        {
            thisChallenge[i].sprite = EnemyLibrary.instance.GetEnemyTypePrefab(roomManager.challengeOfThisRoom.GetTypeOfEnemies()[i]).GetComponent<SpriteRenderer>().sprite;
            thisChallenge[i].preserveAspect = true;
            thisChallenge[i].GetComponentInChildren<Text>().text = "Tier: " + DungeonManager.instance.tierOfEnemies[roomManager.challengeOfThisRoom.GetTypeOfEnemies()[i]];
        }
        challengeBeforeSkip = GameObject.Find("ThisChallenge");
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
