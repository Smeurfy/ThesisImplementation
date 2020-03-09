using UnityEngine;
using Thesis.Enemy;

public class EnemyTimeBasedPerformance : MonoBehaviour 
{
    [SerializeField] private static int maxPerformanceTime = 40; // if the player takes longer than this, the model will give that performance this value.
                                                                    // if the player dies, this value is attributed to that performance as well;
                                                                    // if the player dies, this value is attributed to that performance as well;
    private float timeToClearRoom = -1;     
    private bool isCountingTime = false;
    private EnemyHealthSystem healthSystem;

    public static int GetMaxPerformanceTime() { return maxPerformanceTime; }

    private void Start()
    {
        healthSystem = GetComponent<EnemyHealthSystem>();
        //if(GameManager.instance.IsUsingModel())
        //{
        //    healthSystem.OnEnemyTakeFirstDamage += StartTimer;
        //    healthSystem.OnEnemyDie += StopTimer;
        //}
    }

    private void Update()
    {
        if(isCountingTime)
        {
            timeToClearRoom += Time.fixedDeltaTime;
        }
    }

    private void StartTimer()
    {
        timeToClearRoom = 0;
        isCountingTime = true;
        healthSystem.OnEnemyTakeFirstDamage -= StartTimer;
    }

    private void StopTimer()
    {
        isCountingTime = false;
        timeToClearRoom = Mathf.Clamp(timeToClearRoom, 0, maxPerformanceTime);
        PerformanceData.instance.UpdateTagPerformance(GetComponent<EnemyData>().GetTag(), (int) timeToClearRoom);
        healthSystem.OnEnemyDie -= StopTimer;
    }
}
