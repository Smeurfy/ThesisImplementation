using UnityEngine;

[CreateAssetMenu(fileName = "new type of enemy", menuName = "TypeOfEnemy")]
public class TypeOfEnemy : ScriptableObject
{
    [Header("Tags")]
    [SerializeField] private string enemyTypeTag;
    [Header("Mutation")]
    [SerializeField] private Mutation mutation = null;
    [Header("Controller")]
    [SerializeField] private float attackDistance = -1;
    [SerializeField] private float durationOfPush = -1;
    [Header("Movement")]
    [SerializeField] private float stoppingDistanceToPlayer = -1;
    [SerializeField] private float MovementSpeed = -1;
    [SerializeField] private float stoppingDistanceToPatrolPoint = -1;
    [Header("HealthSystem")]
    [SerializeField] private int healthPoints = -1;
    [Header("Damage")]
    [SerializeField] private float dps = -1;
    [SerializeField] private float secondsBetweenShots = 2f;
    [Header("Performance Penalties")]
    [SerializeField] [Range(5, 10)] private int penaltyMultiplierForGettingShotByEnemy = 7;
    [SerializeField] [Range(5, 10)] private int bonusMultiplierForHittingEnemyOnDeath = 7;
    [Header("Sound FX")]
    [SerializeField] AudioClip[] died;
    [SerializeField] AudioClip[] shot;

    private bool hasEffectOnDeath = false;

    public void MutateStat(StatToMutate statToMutate)
    {
        mutation = EnemyMutationManager.instance.AssignMutation(statToMutate);
        ApplyMutation();
    }

    private void ApplyMutation()
    {
        StatToMutate stat = mutation.GetMutatedStat();

        switch (stat)
        {
            case (StatToMutate.onDeath):
                hasEffectOnDeath = true;
            break;
            case (StatToMutate.hp):
                healthPoints += mutation.GetStatAddition();
            break;
        }
        enemyTypeTag = string.Concat(enemyTypeTag, mutation.GetMutationString() + " mutated");
    }

    internal Color GetMutationColor()
    {
        return mutation.GetMutationColor();
    }

    public string GetEnemyTypeTag() { return enemyTypeTag; }
    
    public float GetAttackDistance() { return attackDistance; }
    public float GetDurationOfPush() { return durationOfPush; }
    
    public int GetHealthPoints() { return healthPoints; }
    public float GetSecondsBetweenShots() { return secondsBetweenShots; }

    public int GetPenaltyForGettingShot() { return penaltyMultiplierForGettingShotByEnemy; }
    public int GetBonusForHittingEnemyOnDeath() { return bonusMultiplierForHittingEnemyOnDeath; }

    public AudioClip GetDiedSoundFX() { return died[Random.Range(0, died.Length)]; }
    public AudioClip GetShotSoundFX() { return shot[Random.Range(0, shot.Length)]; }

    public bool HasOnDeathEffect() { return hasEffectOnDeath; }
}
