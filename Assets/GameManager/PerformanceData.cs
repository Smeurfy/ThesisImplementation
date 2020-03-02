using System;
using System.Collections.Generic;
using Thesis.Enemy;
using UnityEngine;

public enum PerformanceCalculationMethod { basedOnTime, basedOnHits };

public class PerformanceData : MonoBehaviour 
{
    public static PerformanceData instance;

    [SerializeField] private PerformanceCalculationMethod performanceCalculationMethod;

    private readonly int maxNumberOfRecordedPerformancesPerTag = 10;
    private Dictionary<string, Queue<int>> tagsPerformanceHistory;
    [SerializeField] Dictionary<string, int> tagsPerformanceMedian;
    
    private byte minimumPerformanceMedian = 0, maximumPerformanceMedian = 100;

    private void Awake()
    {
        MakeThisObjectSingleton();
    }

    private void Start()
    {
        InitializeTagsPerformance();
        //PrintTagsPerformanceHistory();
    }

    private void Update()       // TODO DELETE THIS FUNCTION, DEBUG ONLY
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            PrintTagsPerformanceHistory();
            PrintTagsPerformanceMedian();
        }
    }

    internal void UpdateTagPerformance(string tag, int performance)
    {
        tagsPerformanceHistory[tag].Enqueue(performance);
        if(JsonWriter.instance != null)
        {
            JsonWriter.instance.WritePerformanceForEnemy(tag, performance);
        }
        CheckIfPerformanceRecordIsFull(tag);
    }
    
    internal void UpdateTagPerformanceMedian()
    {
        int performanceForTag = 0;
        int tagCount = 0;
        Dictionary<string, int> medians = new Dictionary<string, int>();
        byte index = 0;
        foreach(string enemy in tagsPerformanceHistory.Keys)
        {
            foreach (int performance in tagsPerformanceHistory[enemy])
            {
                if (performance != -1)   // sentinel value used in preliminary tests to signal that this enemy was alive when the player died
                {
                    tagCount++;
                    performanceForTag += performance;
                }
            }
            try
            {
                byte medianPerformance = (byte)(performanceForTag / tagCount);
                medians[enemy] = medianPerformance;
                CompareMedianWithMaxAndMinMedians(medianPerformance);
            }
            catch (DivideByZeroException) { }
            index++;
            tagCount = 0;
            performanceForTag = 0;
        }
        foreach(string enemy in medians.Keys)
        {
           tagsPerformanceMedian[enemy] = (int) (Mathf.InverseLerp(minimumPerformanceMedian,
                                                               maximumPerformanceMedian, 
                                                               medians[enemy]) * 100);
 //           print("average for " + enemy + " " + tagsPerformanceMedian[enemy]);

        }
 //       print("minAvg: " + minimumPerformanceMedian + " maxAvg: " + maximumPerformanceMedian);
        minimumPerformanceMedian = 0;
        maximumPerformanceMedian = 100;
        //PrintTagsPerformanceHistory(); // delete
        //PrintTagsPerformanceMedian(); // delete
    }

    private void CompareMedianWithMaxAndMinMedians(byte medianPerformance)
    {
        if (medianPerformance < maximumPerformanceMedian)
        {
            maximumPerformanceMedian = medianPerformance;
        }
        if (medianPerformance > minimumPerformanceMedian)
        {
            minimumPerformanceMedian = medianPerformance;
        }
    }

    private void InitializeTagsPerformance()
    {
        tagsPerformanceHistory = new Dictionary<string, Queue<int>>();
        tagsPerformanceMedian = new Dictionary<string, int>();

        TagManager tagManager = GetComponent<TagManager>();

        /*if(GameManager.instance.IsUsingModel())
        {
            foreach(string tag in tagManager.GetAllTagsToInitialize())
            {
                tagsPerformanceHistory.Add(tag, new Queue<int>());
                tagsPerformanceMedian.Add(tag, -1);
            }
        }
        else
        {*/
            int tagPerformance = -1;
            foreach(string tag in tagManager.GetAllTagsToInitialize())
            {
                tagPerformance = BootstrapTagValues.GetPerformanceForTag(tag);
                tagsPerformanceHistory.Add(tag, new Queue<int>());
                tagsPerformanceHistory[tag].Enqueue(tagPerformance);
                tagsPerformanceMedian.Add(tag, tagPerformance);
            }
            UpdateTagPerformanceMedian();
        //}
    }
    
    internal int GetPerformanceForTag(string tag)
    {
        return tagsPerformanceMedian[tag];
    }

    internal void UpdatePerformanceForAliveEnemies(List<EnemyHealthSystem> enemies)
    {
        foreach(EnemyHealthSystem enemy in enemies)
        {
            if(enemy != null)
            UpdateTagPerformance( enemy.GetComponent<EnemyData>().GetTag(), EnemyTimeBasedPerformance.GetMaxPerformanceTime());
        }
    }

    private void CheckIfPerformanceRecordIsFull(string tag)
    {
        if (tagsPerformanceHistory[tag].Count > maxNumberOfRecordedPerformancesPerTag)
        {
            print("record for " + tag + " was full, overwriting values");
            tagsPerformanceHistory[tag].Dequeue();
        }
    }

    private void PrintTagsPerformanceHistory()
    {
        foreach(string tag in tagsPerformanceHistory.Keys)
        {
            print("performance for " + tag + " median: " + tagsPerformanceMedian[tag]);
            foreach(int performanceValue in tagsPerformanceHistory[tag])
            {
                print(performanceValue);
            }
        }
    }

    private void PrintTagsPerformanceMedian()
    {
        foreach (string tag in tagsPerformanceMedian.Keys)
        {
            print("median performance for " + tag + " is " + tagsPerformanceMedian[tag]);
        }
    }

    public string GetMedianPerformanceValue()
    {
        string medianPerformanceValue = string.Format("Your performance was {0} %", MedianPerformance());
        return medianPerformanceValue;
    }

    private int MedianPerformance()
    {
        int medianSum = 0;
        int numberOfEnemies = 0;
        foreach (string tag in tagsPerformanceMedian.Keys)
        {
            numberOfEnemies++;
            if (tagsPerformanceMedian[tag] >= 0)
            {
                medianSum += tagsPerformanceMedian[tag];
            }
        }
        if (numberOfEnemies > 0)
            return medianSum / numberOfEnemies;
        else
            return 0;
    }
    
    private void MakeThisObjectSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public PerformanceCalculationMethod GetPerformanceCalculationMethod()
    {
        return performanceCalculationMethod;
    }
}
