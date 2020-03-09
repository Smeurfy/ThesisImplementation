using UnityEngine;

public class RoomChallengeGenerator : MonoBehaviour
{
    private RoomManager roomManager;
    private float performanceWeight = -1;
    private float varietyWeight = -1;
    
    private void Start()
    {
        roomManager = GetComponent<RoomManager>();
        //performanceWeight = DungeonManager.instance.GetPerformanceWeight();
        //varietyWeight = DungeonManager.instance.GetNoveltyWeight();
    }
    
    public void GenerateChallengeForNextRoom()
    {
        GeneratePossibleChallengesPopulation();
        //PrintAllChallenges();
       /* print(System.String.Format("room intended novelty = {0}, room intendend performance =  {1}",
                                     roomManager.GetIntendedNoveltyValue(),
                                     roomManager.GetIntendedPerformanceValue()));*/
        //var bestChallenge = PickBestChallengeFromPossiblePopulation();
        //CreateChallengeInGame(bestChallenge);

        //Denis
        var bestChallenge = PickBestChallengeFromPossiblePopulation();
        CreateChallengeInGame(bestChallenge);
    }

    private PossibleChallengeData PickBestChallengeFromPossiblePopulation()
    {
        //print("------------------------");
        //print(roomManager.GetIntendedPerformanceValue());
        PossibleChallengeData bestChallenge = new PossibleChallengeData();
        //int bestChallengesUtility = 0;
        //int possibleChallengesUtility;
        //foreach (PossibleChallengeData possibleChallenge in DungeonManager.instance.possibleChallenges)
        //{
        //    if (IsABetterChallenge(possibleChallenge, bestChallengesUtility, out possibleChallengesUtility))
        //    {
        //        bestChallenge = possibleChallenge;
        //        bestChallengesUtility = possibleChallengesUtility;
        //    }
        //}
        //Denis
        bestChallenge = DungeonManager.instance.possibleChallenges[Random.Range(0, DungeonManager.instance.possibleChallenges.Count)];
        //NoveltyData.instance.UpdateNoveltyModel(bestChallenge);
        //      Debug.Log(string.Format("best utility was: {0}", bestChallengesUtility));        // delete
        // JsonWriter.instance.AddChallengeRunHistory(bestChallenge);
        return bestChallenge;
    }

    //private bool IsABetterChallenge(PossibleChallengeData possibleChallenge, int bestChallengesOverallUtility, out int possibleChallengesOverallProximityToRoom)
    //{
    //    /*float possibleChallengesPerformanceUtility = Mathf.Abs(100 - possibleChallenge.GetPredictedPerformanceValue() -
    //                                                            roomManager.GetIntendedPerformanceValue());*/

    //    //print("room intended: " + roomManager.GetIntendedPerformanceValue() + "challenge predicted: " + possibleChallenge.GetPredictedPerformanceValue());
    //    //print("challenge predicted: " + possibleChallenge.GetPredictedPerformanceValue());
    //    /*float possibleChallengesNoveltyUtility = Mathf.Abs(100 - possibleChallenge.GetNoveltyValue() -
    //                                                       roomManager.GetIntendedNoveltyValue());*/

    //    float possibleChallengesPerformanceProximityToRoom = Mathf.Abs(possibleChallenge.GetPredictedPerformanceValue() -
    //                                                           roomManager.GetIntendedPerformanceValue());
    //    float possibleChallengesNoveltyProximityToRoom = Mathf.Abs(possibleChallenge.GetNoveltyValue() -
    //                                                       roomManager.GetIntendedNoveltyValue());

    //    possibleChallengesOverallProximityToRoom = (int) (possibleChallengesPerformanceProximityToRoom * performanceWeight + 
    //                                        possibleChallengesNoveltyProximityToRoom * varietyWeight);

    //    possibleChallengesOverallProximityToRoom = 100 - possibleChallengesOverallProximityToRoom;

    //    // delete DEBUG ONLY
    //    /*print(System.String.Format("challenge predicted performance = {0}, room's intendend performance =  {1}, challenge's novelty = {2}, room intended novelty = {3}",
    //                        possibleChallenge.GetPredictedPerformanceValue(),
    //                        roomManager.GetIntendedPerformanceValue(),
    //                        possibleChallenge.GetNoveltyValue(),
    //                        roomManager.GetIntendedNoveltyValue()));
    //    print("utility: " + possibleChallengesOverallProximityToRoom);*/
    //    //// delete DEBUG ONLY end
    //    //print("\n");
    //    // delete DEBUG ONLY
    //    //print(System.String.Format("challenge utility = {0}, best was {1}",
    //                        //possibleChallengesOverallUtility,
    //                        //bestChallengesOverallUtility));
    //    // delete DEBUG ONLY end

    //    return possibleChallengesOverallProximityToRoom > bestChallengesOverallUtility ? true : false;
    //}

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
        //print("generating challenge population");
        for (int i = 0; i < DungeonManager.instance.possibleChallenges.Capacity; i++)
        {
            DungeonManager.instance.possibleChallenges[i] = DungeonManager.instance.possibleChallenges[i].GeneratePossibleChallenge();
        }
    }

    private void PrintAllChallenges()
    {
        foreach(PossibleChallengeData possibleChallenge in DungeonManager.instance.possibleChallenges)
        {
            possibleChallenge.PrintPossibleChallengeInfo();
        }
    }
}
