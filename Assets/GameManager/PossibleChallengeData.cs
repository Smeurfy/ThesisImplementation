using UnityEngine;
using System.Collections.Generic;

public class PossibleChallengeData
{
    private TypeOfEnemy[] possibleEnemies;
    public List<List<TypeOfEnemy>> challengesCombination = new List<List<TypeOfEnemy>>();
    internal TypeOfEnemy[] GetTypeOfEnemies() { return possibleEnemies; }
    private bool firstTime = true;
    public bool challengeAvailable = true; 

    private void CreateChallenges()
    {
        for (int i = 0; i < DungeonManager.instance.possibleChallenges.Count; i++)
        {
            for (int j = i; j < EnemyLibrary.instance.GetAllPossibleEnemies().Count; j++)
            {
                var aux = EnemyLibrary.instance.GetSpecificEnemy(i,j);
                //Doesn´t add a challenge where the enemies are the same
                if(aux[0].name != aux[1].name)
                    challengesCombination.Add(EnemyLibrary.instance.GetSpecificEnemy(i,j));
            }
        }
        Debug.Log("done");
        firstTime = false;
    }

    public PossibleChallengeData GeneratePossibleChallenge(int challenge)
    {
        if(firstTime)
            CreateChallenges();
        int numberOfEnemiesInPossibleChallenge = Random.Range(2, 3);
        possibleEnemies = new TypeOfEnemy[numberOfEnemiesInPossibleChallenge];
        PopulatePossibleChallenge(numberOfEnemiesInPossibleChallenge, challenge);
        return this;
    }

    private void PopulatePossibleChallenge(int numberOfEnemiesInPossibleChallenge, int challenge)
    {
        for (int i = 0; i < numberOfEnemiesInPossibleChallenge; i++)
        {
            possibleEnemies[i] = challengesCombination[challenge][i];
        }
    }
        
}
