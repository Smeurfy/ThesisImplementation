using UnityEngine;

[CreateAssetMenu(fileName = "new weapon", menuName = "Weapon")]
public class Weapon : Item 
{
    [SerializeField] private float timeToWaitToShootAgain;
    [SerializeField] private int bulletDamage;
    [SerializeField] private int totalBullets;
    [SerializeField] private TypeOfBullet bulletPrefab; 
    [SerializeField] private Sprite bulletTypeImage; 
    [SerializeField] private Sprite hatImage; 
    [SerializeField] private ParticleSystem fireWeaponPS;
    [SerializeField] private Transform guntip;
    [SerializeField] private AudioClip shotSound;
    
    public float GetTimeToWaitToShootAgain() { return timeToWaitToShootAgain; }

    public int GetBulletDamage() { return bulletDamage; }

    public int GetBulletAmount() { return totalBullets; }

    public TypeOfBullet GetBulletPrefab() { return bulletPrefab; }

    public Sprite GetBulletTypeImage() { return bulletTypeImage; }

    public Sprite GetHatImage() { return hatImage; }

    public ParticleSystem GetFiringParticleSystem() { return fireWeaponPS; }

    public Transform GetGunTip() { return guntip; }

    public AudioClip GetShotSound() { return shotSound; }
}
