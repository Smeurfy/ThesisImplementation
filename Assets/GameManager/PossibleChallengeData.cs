using UnityEngine;
using System.Collections.Generic;

public class PossibleChallengeData
{
    private TypeOfEnemy[] possibleEnemies;
    public List<List<TypeOfEnemy>> challengesCombination = new List<List<TypeOfEnemy>>();
    internal TypeOfEnemy[] GetTypeOfEnemies() { return possibleEnemies; }
    private bool firstTime = true;
    public bool challengeAvailable = true; 
    private bool possibleChallenge = true;

    // private void CreateChallenges()
    // {
    //     for (int i = 0; i < DungeonManager.instance.possibleChallenges.Count; i++)
    //     {
    //         for (int j = i; j < EnemyLibrary.instance.GetAllPossibleEnemies().Count; j++)
    //         {
    //             var aux = EnemyLibrary.instance.GetSpecificEnemy(i,j);
    //             //Doesn´t add a challenge where the enemies are the same
    //             if(aux[0].name != aux[1].name)
    //                 challengesCombination.Add(EnemyLibrary.instance.GetSpecificEnemy(i,j));
    //         }
    //     }
    //     //Debug.Log("done");
    //     firstTime = false;
    // }

    public PossibleChallengeData GeneratePossibleChallenge()
    {
        // if(firstTime)
        //     CreateChallenges();
        int numberOfEnemiesInPossibleChallenge = Random.Range(2, 3);
        possibleEnemies = new TypeOfEnemy[numberOfEnemiesInPossibleChallenge];
        PopulatePossibleChallenge(numberOfEnemiesInPossibleChallenge);
        return this;
    }

    private void PopulatePossibleChallenge(int numberOfEnemiesInPossibleChallenge)
    {
        while(possibleChallenge){
            for (int i = 0; i < numberOfEnemiesInPossibleChallenge; i++)
            {
                possibleEnemies[i] = EnemyLibrary.instance.GetRandomEnemy();
            }
            CheckDifferenceBetweenTiers();
        }
        
    }

    private void CheckDifferenceBetweenTiers(){
        var tierOfEnemies = DungeonManager.instance.tierOfEnemies;
        if(possibleEnemies[0] != possibleEnemies[1] && Mathf.Abs(tierOfEnemies[possibleEnemies[0]] - tierOfEnemies[possibleEnemies[1]]) <= 2)
        {
            possibleChallenge = false;
        }
    }
        
}
