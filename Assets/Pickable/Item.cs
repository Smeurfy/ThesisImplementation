using UnityEngine;

public class Item : ScriptableObject 
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject flyingItemPrefab;
    [SerializeField] private int throwableSpeed = 14;
    [SerializeField] private int damageAsThrowable = 10;
    
    public Sprite GetSprite() { return sprite; }

    public GameObject GetFlyingItemPrefab() { return flyingItemPrefab; }

    public int GetThrowableSpeed() { return throwableSpeed; }

    public int GetThrowableDamage() { return damageAsThrowable; }
}
