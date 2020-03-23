using System;
using System.Collections.Generic;
using UnityEngine;
using Thesis.Enemy;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public event Action RoomCleared = delegate { };
    public event Action OnPlayerEnteredNewRoom = delegate { };
    public enum ConditionToOpen { opened, killAll, pressSwitch, killAllAndPressSwitch }

    [SerializeField] private ConditionToOpen conditionToOpen;
    [SerializeField] private GameObject doorsHolderGameObject;
    [SerializeField] private GameObject enemiesHolderGameObject;
    [SerializeField] private GameObject switchesHolderGameObject;
    [SerializeField] private GameObject itemsHolderGameObject;
    [SerializeField] private GameObject spawnPointsGameObject;
    [SerializeField] private Vector3 nextRoom;
    [SerializeField] private Vector3 playerRoomInitialPosition;

    [SerializeField] private int roomID;
    private int challengesCleared = 0;
    private float secondsBeforeUnfreezing = 1.5f;
    private List<RoomChallenge> typesOfRoom;
    private List<Transform> spawnPointPositions;

    private RoomChallengeGenerator roomChallengeGenerator;
    private DoorManager previousRoomsDoorManager;
    public PossibleChallengeData challengeOfThisRoom;

    //debug purposes
    
    public TypeOfEnemy enem1;
    public TypeOfEnemy enem2;
    public List<TypeOfEnemy> enemy1;
    public List<TypeOfEnemy> enemy2;
    
    #region getters

    public Transform GetEnemyHolder() { return enemiesHolderGameObject.transform; }  // maybe delete, depends if this exists in DungeonManager
    internal int GetRoomID() { return roomID; }
    public Vector3 GetPositionNextRoom() {return nextRoom;}
    public Vector3 GetPlayerRoomInitialPosition() {return playerRoomInitialPosition;}

    #endregion

    public void setPositionNextRoom(Vector3 position) {nextRoom = position;}

    private void Awake()
    {
        playerRoomInitialPosition = GetComponentInParent<Transform>().transform.position - new Vector3(6, 0, 0);
        typesOfRoom = new List<RoomChallenge>();
        GetSpawnPoints();
        roomChallengeGenerator = GetComponent<RoomChallengeGenerator>();
        PlayerHealthSystem.instance.OnPlayerDied += DisableEnemies;
        AfterDeathOptions.instance.OnTryAgain += RepeatChallenge;
    }

    private void Start()
    {
        roomID = DungeonManager.instance.GetRoomID();
        DungeonManager.instance.AssignRoomID();
        if (GetComponent<FirstRoom>())
        {
            AddComponentsBasedOnRoomType();                     // TODO this shouldn't be here, but it needs to be called after Generating the challenge for a room
            SubscribeToTypeOfRoomWinningCondition();
        }
    }

    public void GenerateChallengeForThisRoom()
    {
        roomChallengeGenerator.GenerateChallengeForNextRoom();
        AddComponentsBasedOnRoomType();                     // TODO this shouldn't be here, but it needs to be called after Generating the challenge for a room
        SubscribeToTypeOfRoomWinningCondition();            // TODO this shouldn't be here, but it needs to be called after Generating the challenge for a room
        HideChallenge();                                    // needs to be called after SubscribeToTypeOfRoomWinningCondition
        SubscribeToPlayerEnteringRoom();
    }
    
    private void HideChallenge()
    {
        foreach (Transform enemy in enemiesHolderGameObject.transform)
        {
            try
            {
                enemy.gameObject.GetComponent<EnemyController>().enabled = false;
                enemy.gameObject.GetComponent<Collider2D>().enabled = false;
            }
            catch(MissingComponentException)
            {
                enemy.gameObject.GetComponentInChildren<Collider2D>().enabled = false;
            }
            catch (NullReferenceException)
            {
                enemy.gameObject.GetComponentInChildren<Collider2D>().enabled = false;
            }
        }
    }
    
    private void SubscribeToPlayerEnteringRoom()
    {
        int currentRoomID = roomID;
        previousRoomsDoorManager = DungeonManager.instance.GetRoomManagerByRoomID(--currentRoomID).GetComponentInChildren<DoorManager>(); // the trigger on the previous room's door is the one being used to show the challenge
        previousRoomsDoorManager.OnPlayerEnteredRoom += WaitToShowChallenge;
        previousRoomsDoorManager.OnPlayerEnteredRoom += FreezePlayer;
        previousRoomsDoorManager.OnPlayerEnteredRoom += AnnouncePlayerEnteredRoom;
    }

    private void FreezePlayer()
    {
        PlayerHealthSystem.instance.hpBeforeChallenge = PlayerHealthSystem.instance.GetCurrentHP();
        var player = GameObject.FindGameObjectWithTag("Player");;
        player.transform.position = playerRoomInitialPosition;
        PlayerMovement.characterCanReceiveInput = false;
        StartCoroutine(PlayerCanUpdateAgain());
        previousRoomsDoorManager.OnPlayerEnteredRoom -= FreezePlayer;
    }

    private void WaitToShowChallenge()
    {
        StartCoroutine(ShowChallenge());
        previousRoomsDoorManager.OnPlayerEnteredRoom -= WaitToShowChallenge;
    }

    private void AnnouncePlayerEnteredRoom()
    {
        OnPlayerEnteredNewRoom();
        previousRoomsDoorManager.OnPlayerEnteredRoom -= AnnouncePlayerEnteredRoom;
    }

    private IEnumerator ShowChallenge()
    {
        yield return new WaitForSecondsRealtime(secondsBeforeUnfreezing);
        foreach (Transform enemy in enemiesHolderGameObject.transform)
        {
            enemy.gameObject.GetComponent<EnemyController>().enabled = true;
            enemy.gameObject.GetComponentInChildren<EnemyShoot>().enabled = true;
            try
            {
                enemy.gameObject.GetComponent<Collider2D>().enabled = true;
            }
            catch (MissingComponentException)
            {
                enemy.gameObject.GetComponentInChildren<Collider2D>().enabled = true;
            }
            catch(NullReferenceException)
            {
                enemy.gameObject.GetComponentInChildren<Collider2D>().enabled = true;
            }
        }
    }

    private IEnumerator PlayerCanUpdateAgain()
    {
        yield return new WaitForSecondsRealtime(secondsBeforeUnfreezing);
        PlayerMovement.characterCanReceiveInput = true;
    }

    private void CheckIfAllChallengesHaveBeenOvercome()
    {
        challengesCleared++;
        if (challengesCleared == typesOfRoom.Count)
        {
            RoomCleared();
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        var chosenSpawnPoint = spawnPointPositions[UnityEngine.Random.Range(0, spawnPointPositions.Count)];
        if (UnityEngine.Random.Range(0, 2) == 0)
            spawnPointPositions.Remove(chosenSpawnPoint);
        return chosenSpawnPoint;
    }
    
    private void GetSpawnPoints()
    {
        spawnPointPositions = new List<Transform>();
        foreach (Transform spawnPoint in spawnPointsGameObject.transform.GetComponentsInChildren<Transform>())
        {
            if (spawnPoint != spawnPointsGameObject.transform)
            {
                spawnPointPositions.Add(spawnPoint);
            }
        }
    }

    private void AddComponentsBasedOnRoomType()
    {
        switch (conditionToOpen)
        {
            case (ConditionToOpen.opened):
                RoomCleared();
                break;
            case (ConditionToOpen.killAll):
                gameObject.AddComponent<KillAllChallenge>().InitializeEnemies(enemiesHolderGameObject);
                break;
            case (ConditionToOpen.pressSwitch):
                gameObject.AddComponent<PressSwitchChallenge>().InitializeSwitches(switchesHolderGameObject);
                break;
            case (ConditionToOpen.killAllAndPressSwitch):
                gameObject.AddComponent<KillAllChallenge>().InitializeEnemies(enemiesHolderGameObject);
                gameObject.AddComponent<PressSwitchChallenge>().InitializeSwitches(switchesHolderGameObject);
                break;
        }
    }

    private void SubscribeToTypeOfRoomWinningCondition()
    {
        foreach (RoomChallenge room in GetComponents<RoomChallenge>())
        {
            if (room != this)
            {
                room.RoomCleared += CheckIfAllChallengesHaveBeenOvercome;
                typesOfRoom.Add(room);
                room.SetRoomExits(doorsHolderGameObject);
            }
        }
    }

    public void DisableEnemies()
    {
        foreach (Transform enemy in enemiesHolderGameObject.transform)
        {
            Destroy(enemy.gameObject);
        }
        PlayerHealthSystem.instance.OnPlayerDied -= DisableEnemies;
    }

    public void RepeatChallenge()
    {
        AfterDeathOptions.instance.afterDeathMenu.SetActive(false);
        if(this.roomID == DungeonManager.instance.playersRoom)
        {
            foreach(TypeOfEnemy toe in challengeOfThisRoom.GetTypeOfEnemies())
            {
                var enemyPrefab = EnemyLibrary.instance.GetEnemyTypePrefab(toe);
                GameObject spawnedEnemy = Instantiate(enemyPrefab, GetRandomSpawnPoint().position, Quaternion.identity, this.GetEnemyHolder());
                GameManager.instance.GetComponentInChildren<TierEvolution>().ApplyMutation(spawnedEnemy);
            }
        }
        
    }
}
