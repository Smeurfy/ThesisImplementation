using UnityEngine;

public class HealthSystem : MonoBehaviour 
{
    protected int initialHp;
    protected int currentHp;

    protected void Start()
    {
        currentHp = initialHp;    
    }

    public int GetInitialHp() { return initialHp; }

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
