﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MonstersInfo
{
	public string monsterName;
	public string tier;
    public float numberBullets;
    public float bulletSpeed;
    public float numberOfWaves;
    public float secBtwWaves;
    public float secBtwShots;
    public float angleToShoot;
    public float attackDistance;
    public float stoppingDistance;
    public float movementSpeed;
    public float attackSpeed;


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
	public MonstersInfo(){}

}
