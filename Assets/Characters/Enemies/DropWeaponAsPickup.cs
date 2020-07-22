using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Thesis.Enemy;

public class DropWeaponAsPickup : MonoBehaviour
{
    [SerializeField] private WeaponPickup[] WeaponPickUpPrefabs;
    [SerializeField] private BulletDrop[] bulletDropPrefabs;
    [SerializeField] [Range(0, 100)] private int weaponDropRate;
    [SerializeField] [Range(0, 100)] private int bulletDropRate;

    private Vector3 positionIncrement = new Vector3(-.1f,0,0);  //this vector is needed for the explosions on death to blow the weapon, otherwise their position is the same, and the force vector is 0, so the pickup does not move

    private void Start()
    {
        GetComponent<EnemyHealthSystem>().OnEnemyDie += AttemptDrop;
    }

    private void AttemptDrop()
    {
        byte chance = (byte) Random.Range(0, 100);
        if ( chance < weaponDropRate)
        {
            WeaponAsPickUp();
        }
        else if(chance < bulletDropRate)
        {
            DropBullets();
        }
    }

    private void WeaponAsPickUp()
    {
        int randomIndex = Random.Range(0, WeaponPickUpPrefabs.Length);
        var weaponPickUp = Instantiate(WeaponPickUpPrefabs[randomIndex], transform.position + positionIncrement, Quaternion.identity);  // see positionIncrement decl for explanation 
        var isWeaponPickup = weaponPickUp.GetComponent<WeaponPickup>();
    }

    private void DropBullets()
    {
        int randomIndex = Random.Range(0, bulletDropPrefabs.Length);
        var bulletDrop = Instantiate(bulletDropPrefabs[randomIndex], transform.position, Quaternion.identity);
        bulletDrop.GetComponent<BulletDrop>().EnableCollider(0.5f);
    }
}
