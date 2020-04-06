using UnityEngine;

public class RoomChallengeGenerator : MonoBehaviour
{
    private RoomManager roomManager;

    private void Start()
    {
        roomManager = GetComponent<RoomManager>();
    }

    public void GenerateChallengeForNextRoom()
    {
        // if(DungeonManager.instance.GetFirstTimeGeneratingChallenges())
        //     GeneratePossibleChallengesPopulation();
        // CheckForTierUpdate();
        var bestChallenge = PickBestChallengeFromPossiblePopulation();
        roomManager.challengeOfThisRoom = bestChallenge;
        roomManager.enem1 = roomManager.challengeOfThisRoom.GetTypeOfEnemies()[0];
        roomManager.enem2 = roomManager.challengeOfThisRoom.GetTypeOfEnemies()[1];
        CreateChallengeInGame(bestChallenge);
    }

    // private void CheckForTierUpdate()
    // {
    //     var enemiesCount = EnemyLibrary.instance.GetAllPossibleEnemies().Count;
    //     int tierCount = 0;
    //     foreach (var item in DungeonManager.instance.tierOfEnemies)
    //     {
    //         tierCount += item.Value;
    //     }
    //     if(tierCount != 0 && tierCount % enemiesCount == 0)
    //     {
    //         DungeonManager.instance.IncreaseGlobalTier();
    //     }
    // }

    private PossibleChallengeData PickBestChallengeFromPossiblePopulation()
    {
        var possibleChallengeData = new PossibleChallengeData();
        possibleChallengeData = possibleChallengeData.GeneratePossibleChallenge();

        // PossibleChallengeData bestChallenge = new PossibleChallengeData();
        // foreach(PossibleChallengeData possibleChallenge in DungeonManager.instance.possibleChallenges)
        // {
        //     if(possibleChallenge.challengeAvailable && DungeonManager.instance.tierOfEnemies[possibleChallenge.GetTypeOfEnemies()[0]] == DungeonManager.instance.GetGlobalTier() && 
        //                                                DungeonManager.instance.tierOfEnemies[possibleChallenge.GetTypeOfEnemies()[1]] == DungeonManager.instance.GetGlobalTier())
        //     {
        //         Debug.Log("Devolvo MONSTRTODS");
        //         return possibleChallenge;
        //     }
        // }

        //bestChallenge = DungeonManager.instance.possibleChallenges[Random.Range(0, DungeonManager.instance.possibleChallenges.Count)];
        return possibleChallengeData;
    }

    private void CreateChallengeInGame(PossibleChallengeData bestChallenge)
    {
        var lastSpawnPosition = new Vector3();
        foreach (TypeOfEnemy toe in bestChallenge.GetTypeOfEnemies())
        {
            var randomPosition = roomManager.GetRandomSpawnPoint().position;
            while (lastSpawnPosition == randomPosition)
            {
                randomPosition = roomManager.GetRandomSpawnPoint().position;
            }
            var enemyPrefab = EnemyLibrary.instance.GetEnemyTypePrefab(toe);
            GameObject spawnedEnemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity, roomManager.GetEnemyHolder());
            lastSpawnPosition = randomPosition;
            GameManager.instance.GetComponentInChildren<TierEvolution>().ApplyMutation(spawnedEnemy);
        }
    }

    private void GeneratePossibleChallengesPopulation()
    {



        // for (int i = 0; i < DungeonManager.instance.possibleChallenges.Count; i++)
        //     {
        //         DungeonManager.instance.possibleChallenges[i] = DungeonManager.instance.possibleChallenges[i].GeneratePossibleChallenge(i);
        //         roomManager.enemy1.Add(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[0]);
        //         roomManager.enemy2.Add(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[1]);
        //         if(!DungeonManager.instance.tierOfEnemies.ContainsKey(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[0]))
        //         {
        //             DungeonManager.instance.tierOfEnemies.Add(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[0], 0);
        //             //Debug.Log(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[0]);
        //         }
        //         if(!DungeonManager.instance.tierOfEnemies.ContainsKey(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[1]))
        //         {
        //             DungeonManager.instance.tierOfEnemies.Add(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[1], 0);
        //             //Debug.Log(DungeonManager.instance.possibleChallenges[i].GetTypeOfEnemies()[1]);
        //         }
        //     }
        //     //Debug.Log(DungeonManager.instance.tierOfEnemies.Count + " is the size of the dictionary with the tiers of each enemy")   ;

    }
}
