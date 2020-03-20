using UnityEngine;

public class RespawnWeaponIfOutOfRoom : MonoBehaviour 
{
    public Transform weaponRespawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<BulletManager>())
        {
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            weaponRespawnPoint.GetComponent<ParticleSystem>().Play();
            collision.gameObject.transform.position = weaponRespawnPoint.position;  
        }
    }
}
