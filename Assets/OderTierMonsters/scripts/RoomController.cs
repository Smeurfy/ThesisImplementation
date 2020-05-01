using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomController : MonoBehaviour
{

	public GameObject weapon;
    // Use this for initialization
    void Start()
    {
        PlayerHealthSystemTest.instance.OnPlayerDied += RestartRoom;
    }

    void RestartRoom()
    {
		ClearBullets();
		StartCoroutine(ResetPlayer());	
        StartCoroutine(ResetMonster());
    }

    IEnumerator ResetPlayer()
    {
		yield return new WaitForSecondsRealtime(2);
		PlayerHealthSystemTest.instance.EnablePlayer();
    }

	IEnumerator ResetMonster(){
		var enemy = GameObject.FindObjectOfType<Thesis.Enemy.EnemyHealthSystemTest>().gameObject;
		enemy.SetActive(false);
		yield return new WaitForSecondsRealtime(2);
		var obj = Instantiate(enemy, GameObject.Find("roomTest").transform.position + new Vector3(3, 0, 0), Quaternion.identity, GameObject.Find("roomTest").GetComponentInChildren<RoomManager>().GetEnemyHolder());

		Instantiate(weapon, weapon.transform.position, Quaternion.identity);

		obj.SetActive(true);
		obj.name = enemy.name;
		Destroy(enemy);
	}

	void ClearBullets()
    {
        var bullets = GameObject.FindObjectsOfType<RegularBullet>();
        foreach (var item in bullets)
        {
            Destroy(item.gameObject);
        }
    }
}
