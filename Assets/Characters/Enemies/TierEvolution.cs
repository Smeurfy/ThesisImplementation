using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierEvolution : MonoBehaviour
{

	[SerializeField] private int increaseNumberOfBullets;
    [SerializeField] private int increaseNumberOfWaves;
    [SerializeField] private int increaseBulletSpeed;
    [SerializeField] private int increaseMovSpeed;
    [SerializeField] private int iceZombieAttackSpeed;


	public void ApplyMutation(GameObject enemyPrefab)
	{
		var enemyTier = DungeonManager.instance.tierOfEnemies[enemyPrefab.GetComponent<EnemyData>().GetTypeOfEnemy()];
		
		if(enemyTier >= 1 && enemyPrefab.GetComponent<EnemyData>().GetTypeOfEnemy().name != "iceZombie"){
			//Debug.Log(enemyPrefab.GetComponentInChildren<BulletSpawner>().numberOfBullets + " antes bullets " + enemyPrefab.name);
			enemyPrefab.GetComponentInChildren<BulletSpawner>().numberOfBullets += increaseNumberOfBullets;
			//Debug.Log(enemyPrefab.GetComponentInChildren<BulletSpawner>().numberOfBullets + " depois bullets " + enemyPrefab.name);
		}
		if(enemyTier >= 2){
			if(enemyPrefab.GetComponent<EnemyData>().GetTypeOfEnemy().name == "iceZombie")
			{
				enemyPrefab.GetComponent<IceZombiePhysicalAttack>().attackFlightSpeed += iceZombieAttackSpeed;
			}
			else
			{
				//Debug.Log(enemyPrefab.GetComponentInChildren<BulletSpawner>().numberOfWaves + " antes waves " + enemyPrefab.name);
				enemyPrefab.GetComponentInChildren<BulletSpawner>().numberOfWaves += increaseNumberOfWaves;
				//Debug.Log(enemyPrefab.GetComponentInChildren<BulletSpawner>().numberOfWaves + " depois waves " + enemyPrefab.name);
			}	
		}
		if(enemyTier >= 3 && enemyPrefab.GetComponent<EnemyData>().GetTypeOfEnemy().name != "iceZombie"){
			//Debug.Log(enemyPrefab.GetComponentInChildren<BulletSpawner>().bulletSpeed + " antes bulletSpeed " + enemyPrefab.name);
			enemyPrefab.GetComponentInChildren<BulletSpawner>().bulletSpeed += increaseBulletSpeed;
			//Debug.Log(enemyPrefab.GetComponentInChildren<BulletSpawner>().bulletSpeed + " depois bulletSpeed " + enemyPrefab.name);
		}
		if(enemyTier >= 4){
			//Debug.Log(enemyPrefab.GetComponent<Thesis.Enemy.EnemyMovement>().movementSpeed + " antes movementSpeed " + enemyPrefab.name);
			enemyPrefab.GetComponent<Thesis.Enemy.EnemyMovement>().movementSpeed += increaseMovSpeed;
			//Debug.Log(enemyPrefab.GetComponent<Thesis.Enemy.EnemyMovement>().movementSpeed + " depois movementSpeed " + enemyPrefab.name);
		}		
	}
}
