using System.Collections.Generic;
using Thesis.Enemy;
using UnityEngine;

public class KillAllChallenge : RoomChallenge
{
    private List<EnemyHealthSystem> enemies;
    private int enemiesKilled = 0;
    private int totalNumberOfEnemies;

    private void Start()
    {
        if(GameManager.instance.IsUsingModel())
            GameManager.instance.GetPlayerReference().GetComponent<PlayerHealthSystem>().OnPlayerDied += UpdatePerformanceForAliveEnemies;
    }

    private void UpdatePerformanceForAliveEnemies()
    {
        PerformanceData.instance.UpdatePerformanceForAliveEnemies(enemies);
    }

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
            GameManager.instance.GetPlayerReference().GetComponent<PlayerHealthSystem>().OnPlayerDied -= UpdatePerformanceForAliveEnemies;
            Destroy(this);
        }
    }

    private void EnemyWasKilled()
    {
        enemiesKilled++;
    }
}
