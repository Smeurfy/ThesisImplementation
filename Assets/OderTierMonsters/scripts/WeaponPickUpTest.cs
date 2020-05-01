using UnityEngine;
using UnityEngine.UI;

public class WeaponPickUpTest : MonoBehaviour 
{
    [SerializeField] private Weapon weaponToBePickedUp;
    [SerializeField] private Image typeOfBullet;

    #region getters
    public Weapon GetWeapon() { return weaponToBePickedUp; }
    #endregion

    public void SetWeapon(Weapon weaponToSet)
    {
        weaponToBePickedUp = weaponToSet;
        GetComponent<SpriteRenderer>().sprite = weaponToBePickedUp.GetSprite();
        typeOfBullet.sprite = weaponToBePickedUp.GetBulletTypeImage();
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        var playerShoot = collision.gameObject.GetComponentInChildren<PlayerShootTest>();
        
        if(playerShoot)
        {
            if (!playerShoot.IsPlayerHoldingThrowable())
            {
                playerShoot.ThrowablePickup(gameObject);
                Destroy(gameObject);
            }
        }
    }

}
