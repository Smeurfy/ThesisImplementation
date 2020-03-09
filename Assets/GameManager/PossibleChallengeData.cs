using UnityEngine;

public class PossibleChallengeData 
{
    private TypeOfEnemy[] possibleEnemies;


    internal TypeOfEnemy[] GetTypeOfEnemies() { return possibleEnemies; }

    public PossibleChallengeData GeneratePossibleChallenge()
    {
        int numberOfEnemiesInPossibleChallenge = Random.Range(2, 3);
        possibleEnemies = new TypeOfEnemy[numberOfEnemiesInPossibleChallenge];
        PopulatePossibleChallenge(numberOfEnemiesInPossibleChallenge);
        return this;
    }

    private void PopulatePossibleChallenge(int numberOfEnemiesInPossibleChallenge)
    {
        for (int i = 0; i < numberOfEnemiesInPossibleChallenge; i++)
        {
            possibleEnemies[i] = EnemyLibrary.instance.GetRandomEnemy();
        }
    }
        
}
