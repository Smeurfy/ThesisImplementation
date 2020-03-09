using UnityEngine;

public class RoomChallengeGenerator : MonoBehaviour
{
    private RoomManager roomManager;
    private bool firstTimeGeneratingChallenges = true;
    
    private void Start()
    {
        roomManager = GetComponent<RoomManager>();

    }
    
    public void GenerateChallengeForNextRoom()
    {
        GeneratePossibleChallengesPopulation();
        var bestChallenge = PickBestChallengeFromPossiblePopulation();
        CreateChallengeInGame(bestChallenge);
    }

    private PossibleChallengeData PickBestChallengeFromPossiblePopulation()
    {

        PossibleChallengeData bestChallenge = new PossibleChallengeData();
        //gets the challenge based on the room the player is
        bestChallenge = DungeonManager.instance.possibleChallenges[roomManager.GetRoomID() % DungeonManager.instance.GetNumberOfMecanics()];
        return bestChallenge; 
    }

    private void CreateChallengeInGame(PossibleChallengeData bestChallenge)
    {
        foreach(TypeOfEnemy toe in bestChallenge.GetTypeOfEnemies())
        {
            var enemyPrefab = EnemyLibrary.instance.GetEnemyTypePrefab(toe);
            GameObject spawnedEnemy = Instantiate(enemyPrefab, roomManager.GetRandomSpawnPoint().position, Quaternion.identity, roomManager.GetEnemyHolder());
        }
    }

    private void GeneratePossibleChallengesPopulation()
    {
        //only generates the challenges one time
        if(firstTimeGeneratingChallenges)
        {
            for (int i = 0; i < DungeonManager.instance.possibleChallenges.Capacity; i++)
            {
                DungeonManager.instance.possibleChallenges[i] = DungeonManager.instance.possibleChallenges[i].GeneratePossibleChallenge();
            }
        }
        
    }
}
