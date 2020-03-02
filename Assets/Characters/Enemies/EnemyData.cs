using UnityEngine;

public class EnemyData : MonoBehaviour 
{
    [SerializeField] private TypeOfEnemy typeOfEnemy;


    public void SetEnemyType(TypeOfEnemy type)
    {
        typeOfEnemy = type;
        if (typeOfEnemy.GetEnemyTypeTag() != "target")
        {
            InitializePerformanceCalculationMethod();
        }
    }
    
    private void InitializePerformanceCalculationMethod()
    {
        var performanceCalculationMethod = PerformanceData.instance.GetPerformanceCalculationMethod();
        if(performanceCalculationMethod == PerformanceCalculationMethod.basedOnHits)
        {
            gameObject.AddComponent<EnemyPerformanceData>();
        }
    }

    public TypeOfEnemy GetTypeOfEnemy() { return typeOfEnemy; }
    
    public int GetInitialHealth() { return typeOfEnemy.GetHealthPoints(); }

    public float GetSecondsBetweenShots() { return typeOfEnemy.GetSecondsBetweenShots(); }

    public string GetTag() { return typeOfEnemy.GetEnemyTypeTag(); }

    public int GetPenaltyForGettingShot() { return typeOfEnemy.GetPenaltyForGettingShot(); }

    public int GetBonusForHittingEnemyOnDeath() { return typeOfEnemy.GetBonusForHittingEnemyOnDeath(); }
    
    public AudioClip GetDiedSoundFX() { return typeOfEnemy.GetDiedSoundFX(); }

    public AudioClip GetShotSoundFX() { return typeOfEnemy.GetShotSoundFX(); }
    
    internal Color GetMutationColor() { return typeOfEnemy.GetMutationColor(); }

    public bool HasOnDeathEffect() { return typeOfEnemy.HasOnDeathEffect(); }
}
