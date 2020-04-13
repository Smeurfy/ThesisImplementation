using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersInfo : MonoBehaviour
{
	string monsterName;
	string tier;
    float numberBullets;
    float bulletSpeed;
    float numberOfWaves;
    float secBtwWaves;
    float secBtwShots;
    float angleToShoot;
    float attackDistance;
    float stoppingDistance;
    float movementSpeed;
    float attackSpeed;


    public MonstersInfo(string monsterName, string tier, float numberBullets, float bulletSpeed, float numberOfWaves, float secBtwWaves, float secBtwShots, float angleToShoot,
                float attackDistance, float stoppingDistance, float movementSpeed, float attackSpeed)
    {
		this.monsterName = monsterName;
		this.tier = tier;
		this.numberBullets = numberBullets;
		this.bulletSpeed = bulletSpeed;
		this.numberOfWaves = numberOfWaves;
		this.secBtwWaves = secBtwWaves;
		this.secBtwShots = secBtwShots;
		this.angleToShoot = angleToShoot;
		this.attackDistance = attackDistance;
		this.stoppingDistance = stoppingDistance;
		this.movementSpeed = movementSpeed;
		this.attackSpeed = attackSpeed;
    }

}
