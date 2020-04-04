using System.Collections.Generic;
using Thesis.Enemy;
using UnityEngine;

public class KillAllChallenge : RoomChallenge
{
    private List<EnemyHealthSystem> enemies;
    [SerializeField]public int enemiesKilled = 0;
    [SerializeField]public int totalNumberOfEnemies;


    private void Update()
    {
        CheckIfAllEnemiesAreDead();
    }

    public void InitializeEnemies(GameObject enemiesHolder)
    {
        enemies = new List<EnemyHealthSystem>();
        foreach (EnemyHealthSystem enemy in enemiesHolder.transform.GetComponentsInChildren<EnemyHealthSystem>())
        {
            enemies.Add(enemy);
            enemy.OnEnemyDie += EnemyWasKilled;
            totalNumberOfEnemies++;
        }
    }
    
    private void CheckIfAllEnemiesAreDead()
    {
        if (enemiesKilled == totalNumberOfEnemies)
        {
            ChallengeComplete();
            Destroy(this);
        }
    }

    private void EnemyWasKilled()
    {
        enemiesKilled++;
    }

}
