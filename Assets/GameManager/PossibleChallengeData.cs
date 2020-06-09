using UnityEngine;
using System.Collections.Generic;

public class PossibleChallengeData
{
    private TypeOfEnemy[] possibleEnemies;
    internal TypeOfEnemy[] GetTypeOfEnemies() { return possibleEnemies; }
    private bool firstTime = true;
    public bool challengeAvailable = true;
    private bool possibleChallenge = true;

    private List<PossibleChallengeData> _challenges;
    private Dictionary<TypeOfEnemy, int> _enemyTiers;

    public PossibleChallengeData()
    {
        _challenges = DungeonManager.instance._finalChallenges;
        _enemyTiers = DungeonManager.instance.tierOfEnemies;
    }

    public PossibleChallengeData GeneratePossibleChallenge()
    {
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
            // if (DungeonManager.instance.DungeonBeaten())
            // {
            //     possibleChallenge = false;
            // }
        }

    }

    private void CheckChallenge()
    {
        if (DifferentEnemies() && !EqualChallengeAsPrevious() && DifferenceBtwMaxMinTier())
        {
            
            if (_enemyTiers[possibleEnemies[0]] == 5 || _enemyTiers[possibleEnemies[1]] == 5)
            {
                DungeonManager.instance._maxDifBewTiers--;
            }
            else
            {
                possibleChallenge = false;
            }
        }
    }

    private bool DifferenceBtwMaxMinTier()
    {
        int max = 0;
        int min = 5;
        foreach (var item in _enemyTiers)
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
        
        if ((max - min) > DungeonManager.instance._maxDifBewTiers)
        {
            int sum = 0;
            foreach (var item in _enemyTiers)
            {
                if (item.Value == min)
                {
                    sum++;
                }
            }
            if (sum >= 2)
            {
                if (_enemyTiers[possibleEnemies[0]] == min && _enemyTiers[possibleEnemies[1]] == min)
                {
                    return true;
                }
            }
            else
            {
                if (_enemyTiers[possibleEnemies[0]] == min || _enemyTiers[possibleEnemies[1]] == min)
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
        if (_challenges.Count > 1)
        {
            TypeOfEnemy[] previousRoomChallenge = _challenges[(_challenges.Count - 2)].GetTypeOfEnemies();
            if (previousRoomChallenge[0] == possibleEnemies[0] || previousRoomChallenge[0] == possibleEnemies[1] ||
                previousRoomChallenge[1] == possibleEnemies[0] || previousRoomChallenge[1] == possibleEnemies[1])
            {
                return true;
            }
            return false;
        }
        return false;



        // This is for when the selection of a challenge was done in each room
        // if (DungeonManager.instance.playersRoom != -1 && DungeonManager.instance.playersRoom != 0)
        // {
        //     var previousRoomPlayer = DungeonManager.instance.playersRoom - 1;
        //     var previousRoomChallenge = DungeonManager.instance.GetRoomManagerByRoomID(previousRoomPlayer).challengeOfThisRoom.GetTypeOfEnemies();
        //     if (previousRoomChallenge[0] == possibleEnemies[0] || previousRoomChallenge[0] == possibleEnemies[1] ||
        //         previousRoomChallenge[1] == possibleEnemies[0] || previousRoomChallenge[1] == possibleEnemies[1])
        //     {
        //         Debug.Log("Same Challenge");
        //         return true;
        //     }
        //     Debug.Log("Diff Challenge");
        //     return false;
        // }
        // return false;

    }

}
