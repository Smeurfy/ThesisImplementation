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
        while (possibleChallenge)
        {
            for (int i = 0; i < numberOfEnemiesInPossibleChallenge; i++)
            {
                possibleEnemies[i] = EnemyLibrary.instance.GetRandomEnemy();
            }
            CheckChallenge();
            if (DungeonManager.instance.DungeonBeaten())
            {
                possibleChallenge = false;
            }
        }

    }

    private void CheckChallenge()
    {
        var tierOfEnemies = DungeonManager.instance.tierOfEnemies;

        if (DifferentEnemies() && !EqualChallengeAsPrevious() && CheckDifferenceBetweenTiers() && DifferenceBtwMaxMinTier())
        {
            possibleChallenge = false;
        }
    }

    private bool DifferenceBtwMaxMinTier()
    {
        var tierOfEnemies = DungeonManager.instance.tierOfEnemies;
        int max = 0;
        int min = 5;
        foreach (var item in tierOfEnemies)
        {
            if (item.Value < min)
            {
                min = item.Value;
            }
            if (item.Value > max)
            {
                max = item.Value;
            }
        }
        Debug.Log(min + " é o minimo");
        Debug.Log(max + " é o maximo");
        if (max - min > 1)
        {
            int sum = 0;
            foreach (var item in tierOfEnemies)
            {
                if (item.Value == min)
                {
                    sum++;
                }
            }
            if (sum >= 2)
            {
                if (tierOfEnemies[possibleEnemies[0]] == min && tierOfEnemies[possibleEnemies[1]] == min)
                {
                    return true;
                }
            }
            else
            {
                if (tierOfEnemies[possibleEnemies[0]] == min || tierOfEnemies[possibleEnemies[1]] == min)
                {
                    return true;
                }
            }

            return false;
        }
        return true;

    }

    private bool DifferentEnemies()
    {
        return possibleEnemies[0] != possibleEnemies[1];
    }

    private bool CheckDifferenceBetweenTiers()
    {
        var tierOfEnemies = DungeonManager.instance.tierOfEnemies;
        return Mathf.Abs(tierOfEnemies[possibleEnemies[0]] - tierOfEnemies[possibleEnemies[1]]) <= 2;
    }

    private bool CheckIfChallengeSkipped()
    {
        var skipedChallenges = DungeonManager.instance.skipedChallenges;
        foreach (var item in skipedChallenges)
        {
            if ((item[0] == possibleEnemies[0] && item[1] == possibleEnemies[1]) || (item[1] == possibleEnemies[0] && item[0] == possibleEnemies[1]))
            {
                return true;
            }
        }
        return false;
    }

    private bool EqualChallengeAsPrevious()
    {
        if (DungeonManager.instance.playersRoom != -1 && DungeonManager.instance.playersRoom != 0)
        {
            var previousRoomPlayer = DungeonManager.instance.playersRoom - 1;
            var previousRoomChallenge = DungeonManager.instance.GetRoomManagerByRoomID(previousRoomPlayer).challengeOfThisRoom.GetTypeOfEnemies();
            if (previousRoomChallenge[0] == possibleEnemies[0] || previousRoomChallenge[0] == possibleEnemies[1] ||
                previousRoomChallenge[1] == possibleEnemies[0] || previousRoomChallenge[1] == possibleEnemies[1])
            {
                Debug.Log("Same Challenge");
                return true;
            }
            Debug.Log("Diff Challenge");
            return false;
        }
        return false;

    }

}
