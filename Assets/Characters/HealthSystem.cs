using UnityEngine;

public class HealthSystem : MonoBehaviour 
{
    protected int initialHp;
    protected int currentHp;

    public int hpBeforeChallenge;

    protected void Start()
    {
        currentHp = initialHp;
        hpBeforeChallenge = initialHp;
    }

    public int GetCurrentHP() { return currentHp; }

    public virtual void TakeDamage(int damageToTake)
    {
        currentHp -= damageToTake;
        
        bool isDamageFatal = currentHp <= 0;
        if(isDamageFatal)
        {
            CharacterDied();
        }
    }

    internal virtual void CharacterDied() { }
}
