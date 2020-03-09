using UnityEngine;
using Thesis.Enemy;

public class EnemyPerformanceData : MonoBehaviour 
{
    private int penaltyMultiplierForGettingShotByEnemy;
    private int bonusMultiplierForHittingEnemyOnDeath;
    private int damageFromItems = 0;
    private int timesHittedByItems = 0;
    private int damageFromBullets = 0;
    private int timesHittedByBullets = 0;
    private int timesPlayerWasShot = 0;
    private EnemyHealthSystem enemyHealth;
    private string enemyTag;
    private PlayerHealthSystem playerHealth;

    private void Start()
    {
        InitializeClassVariables();
        //if(GameManager.instance.IsUsingModel())
        //{
        //    SubscribeToEnemyTakingDamageEvents();
        //    SubscribeToPlayerDyingEvent();
        //}
    }

    private void DamagedByBullet(int damageToTake)
    {
        timesHittedByBullets++;
        damageFromBullets += damageToTake;
    }

    private void DamagedByItem(int damageToTake)
    {
        timesHittedByItems++;
        damageFromItems += damageToTake;
    }

    internal void PlayerWasShot()
    {
        timesPlayerWasShot++;
    }

    private void UpdatePlayerPerformanceDataForDeadEnemy()
    {
        int performance = CalculatePerformanceForDeadEnemy();
        PerformanceData.instance.UpdateTagPerformance(enemyTag, performance);
        playerHealth.OnPlayerDied -= UpdatePlayerPerformanceDataForDeadPlayer;
    }

    private void UpdatePlayerPerformanceDataForDeadPlayer()
    {
        int performance = CalculatePerformanceForDeadPlayer();
        PerformanceData.instance.UpdateTagPerformance(enemyTag, performance);
    }

    private int CalculatePerformanceForDeadEnemy()
    {
        int performanceWhenEnemyDies = 100 - timesPlayerWasShot * penaltyMultiplierForGettingShotByEnemy;
        performanceWhenEnemyDies = Mathf.Clamp(performanceWhenEnemyDies, 55, 100);
        print("performance updated for dead enemy " + enemyTag + ": " + performanceWhenEnemyDies);
        return performanceWhenEnemyDies;
    }

    private int CalculatePerformanceForDeadPlayer()
    {
        int performanceWhenPlayerDies = (timesHittedByBullets + timesHittedByItems) * bonusMultiplierForHittingEnemyOnDeath;
        performanceWhenPlayerDies = Mathf.Clamp(performanceWhenPlayerDies, 0, 40);
        print("performance updated for dead player " + enemyTag + ": " + performanceWhenPlayerDies);
        return performanceWhenPlayerDies;
    }
    
    private void SubscribeToEnemyTakingDamageEvents()
    {
        enemyHealth.OnEnemyTakeDamageFromBullet += DamagedByBullet;
        enemyHealth.OnEnemyTakeDamageFromItem += DamagedByItem;
        enemyHealth.OnEnemyDie += UpdatePlayerPerformanceDataForDeadEnemy;
    }

    private void SubscribeToPlayerDyingEvent()
    {
        playerHealth.OnPlayerDied += UpdatePlayerPerformanceDataForDeadPlayer;
    }

    private void InitializeClassVariables()
    {
        var enemyData = GetComponent<EnemyData>();
        enemyHealth = GetComponent<EnemyHealthSystem>();
        enemyTag = enemyData.GetTag();
        playerHealth = GameManager.instance.GetPlayerReference().GetComponent<PlayerHealthSystem>();
        penaltyMultiplierForGettingShotByEnemy = enemyData.GetPenaltyForGettingShot();
        bonusMultiplierForHittingEnemyOnDeath = enemyData.GetBonusForHittingEnemyOnDeath();
    }

}
