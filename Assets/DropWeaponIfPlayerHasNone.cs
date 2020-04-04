using UnityEngine;

public class DropWeaponIfPlayerHasNone : MonoBehaviour 
{
    [SerializeField] GameObject weaponToDrop;
    [SerializeField] Transform weaponSpawnTransform;

    private bool hasAnyWeaponEnteredTheRoom = false;
    

    // if a weapon is thrown to the room or the player comes in with a weapon, nothing happens, if neither of this scenarios is true, then we drop a weapon
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<TypeOfBullet>())
        {
            Destroy(collision.gameObject);
            return;
        }
        if(collision.GetComponent<BulletManager>() || collision.gameObject.GetComponentInChildren<PlayerShoot>().IsPlayerHoldingThrowable())
        {
            Destroy(gameObject);
        }
        else
        {
            //Trigger runs before the update of the var playersRoom so kinda hammered
            int aux = DungeonManager.instance.playersRoom;
            aux++;
            Instantiate(weaponToDrop, DungeonManager.instance.GetRoomManagerByRoomID(aux).GetPlayerRoomInitialPosition(), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
