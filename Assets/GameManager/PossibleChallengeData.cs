using UnityEngine;

public class PossibleChallengeData 
{
    private int totalHealthPool = 0;
    private int predictedPerformanceValue = -1;
    private int noveltyValue = -1;
    private TypeOfEnemy[] possibleEnemies;
    [Range(0, 1)] private float performanceWeight = .65f;

    #region getters
    public int GetPredictedPerformanceValue () { return predictedPerformanceValue; }
    public int GetNoveltyValue() { return noveltyValue; }
    #endregion

    internal TypeOfEnemy[] GetTypeOfEnemies() { return possibleEnemies; }

    public PossibleChallengeData GeneratePossibleChallenge()
    {
        int numberOfEnemiesInPossibleChallenge = Random.Range(2, 5);
        possibleEnemies = new TypeOfEnemy[numberOfEnemiesInPossibleChallenge];
        PopulatePossibleChallenge(numberOfEnemiesInPossibleChallenge);
        UpdatePredictedPerformanceValue();
        UpdateVarietyValue();
        return this;
    }

    private void UpdatePredictedPerformanceValue()
    {
        int numberOfTagsInChallenge = 0;
        int predictedPerformanceSum = 0;
        int averagePredictedPerformance = 0;
        int numberOfEnemiesAsPercentage = 0;
        foreach(TypeOfEnemy enemy in possibleEnemies)
        {
            //Debug.Log("\n" + enemy.GetEnemyTypeTag());
            numberOfTagsInChallenge++;
            predictedPerformanceSum += PerformanceData.instance.GetPerformanceForTag(enemy.GetEnemyTypeTag());
        }
        averagePredictedPerformance = predictedPerformanceSum / numberOfTagsInChallenge;

        numberOfEnemiesAsPercentage = (int)(Mathf.InverseLerp(4, 2, numberOfTagsInChallenge) * 100);

        predictedPerformanceValue  = (int) (averagePredictedPerformance * performanceWeight) +  
                                     (int) (numberOfEnemiesAsPercentage * (1 - performanceWeight));
        /*Debug.Log("avgPredic: " + averagePredictedPerformance +
                    "\t enemies as %: " + numberOfEnemiesAsPercentage +
                    "\t predicted total: " + predictedPerformanceValue);*/
        predictedPerformanceValue = Mathf.Clamp(predictedPerformanceValue, 0, 100);
    }
    
    private void UpdateVarietyValue()
    {
        int numberOfTagsInChallenge = 0;
        int noveltySum = 0;
        foreach(TypeOfEnemy enemy in possibleEnemies)
        {
            numberOfTagsInChallenge++;
            noveltySum += NoveltyData.instance.GetNoveltyForTag(enemy.GetEnemyTypeTag());
        }
        noveltyValue = noveltySum / numberOfTagsInChallenge;
        //Debug.Log("nov: " + noveltyValue);
    }
    private void PopulatePossibleChallenge(int numberOfEnemiesInPossibleChallenge)
    {
        for (int i = 0; i < numberOfEnemiesInPossibleChallenge; i++)
        {
            possibleEnemies[i] = EnemyLibrary.instance.GetRandomEnemy();
            totalHealthPool += possibleEnemies[i].GetHealthPoints();
        }
    }
        
    public void PrintPossibleChallengeInfo()
    {
        //Debug.Log("Total health pool: " + totalHealthPool + " with " + possibleEnemies.Length + " enemies:");
        foreach(TypeOfEnemy toe in possibleEnemies)
        {
            Debug.Log(toe.GetEnemyTypeTag());
        }
        Debug.Log(" and predicted performance of " + predictedPerformanceValue + " and novelty of: " + noveltyValue);
    }
}
