using UnityEngine;
using System.Collections.Generic;

public class RoomChallengeGenerator : MonoBehaviour
{
    private RoomManager roomManager;

    private void Start()
    {
        roomManager = GetComponent<RoomManager>();
    }

    public void GenerateChallengeForNextRoom()
    {
        var bestChallenge = PickBestChallengeFromPossiblePopulation();
        roomManager.challengeOfThisRoom = bestChallenge;
        roomManager.enem1 = roomManager.challengeOfThisRoom.GetTypeOfEnemies()[0];
        roomManager.enem2 = roomManager.challengeOfThisRoom.GetTypeOfEnemies()[1];
        CreateChallengeInGame(bestChallenge);
    }

    private PossibleChallengeData PickBestChallengeFromPossiblePopulation()
    {
        var possibleChallengeData = DungeonManager.instance._finalChallenges[DungeonManager.instance.indexChallenge];
        return possibleChallengeData;
    }

    public void CreateChallengeInGame(PossibleChallengeData bestChallenge)
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
            spawnedEnemy.name = enemyPrefab.name;
            lastSpawnPosition = randomPosition;
            ApplyCharacteristics(spawnedEnemy, toe);
        }
    }

    private void ApplyCharacteristics(GameObject enemy, TypeOfEnemy typeOfEnemy)
    {
        var enemyTier = DungeonManager.instance.tierOfEnemies[typeOfEnemy];
        string tierName = "";
        //Convert 0,1,2,3,4,5 to default, Tier1, Tier2 as in the loaded json file
        if (enemyTier == 0 || enemyTier == 1)
            tierName = "default";
        if (enemyTier == 2 || enemyTier == 3)
            tierName = "Tier1";
        if (enemyTier == 4)
            tierName = "Tier2";
        var mInfo = DungeonManager.instance.monstersInfo[enemy.name][tierName];
        if (enemy.name == "iceZombieTest")
        {
            enemy.GetComponent<IceZombieController>().timeBetweenAttacksInSecs = mInfo.timeBetweenAttacks;
            enemy.GetComponent<IceZombiePhysicalAttack>().DurationOfAttackInSecs = mInfo.durationOfAttacks;
            enemy.GetComponent<IceZombiePhysicalAttack>().attackFlightSpeed = mInfo.attackSpeed;
            enemy.GetComponent<Thesis.Enemy.EnemyMovement>().stoppingDistanceToPlayer = mInfo.stoppingDistance;
            enemy.GetComponent<Thesis.Enemy.EnemyMovement>().movementSpeed = mInfo.movementSpeed;
        }
        else
        {
            var enemyCharac = enemy.GetComponentInChildren<BulletSpawner>();
            enemyCharac.numberOfBullets = (int)mInfo.numberBullets;
            enemyCharac.bulletSpeed = (int)mInfo.bulletSpeed;
            enemyCharac.numberOfWaves = (int)mInfo.numberOfWaves;
            enemyCharac.secondsBetweenWaves = (int)mInfo.secBtwWaves;
            enemyCharac.secondsBetweenShots = mInfo.secBtwShots;
            enemyCharac.angleToShootInDegrees = mInfo.angleToShoot;
            enemy.GetComponent<Thesis.Enemy.EnemyController>().attackDistance = mInfo.attackDistance;
            enemy.GetComponent<Thesis.Enemy.EnemyMovement>().stoppingDistanceToPlayer = mInfo.stoppingDistance;
            enemy.GetComponent<Thesis.Enemy.EnemyMovement>().movementSpeed = mInfo.movementSpeed;
            enemy.GetComponentInChildren<Thesis.Enemy.EnemyShoot>().timeToWaitBeforeShootingAgain = mInfo.attackSpeed;
        }
    }
}
