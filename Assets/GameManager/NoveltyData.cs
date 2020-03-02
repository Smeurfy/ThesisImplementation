using System.Collections.Generic;
using UnityEngine;

public class NoveltyData : MonoBehaviour 
{
    public static NoveltyData instance;

    private Dictionary<string, int> tagNoveltyValues;
    private Dictionary<string, int> tagUsageCount;
    private int totalNumberOfTagsGenerated  = 0;

    private int minimumNoveltyMedian = 0, maximumNoveltyMedian = 100;
    private void Start()
    {
        MakeThisObjectSingleton();
        InitializeTagsNoveltyAndUsageCount();
        //PrintTagsNovelty();    // delete, debug only
        //PrintNoveltyCounts();  // delete, debug only
    }

    public int GetNoveltyForTag(string tag)
    {
        return tagNoveltyValues[tag];
    }
    
    internal void UpdateNoveltyModel(PossibleChallengeData challengeToCreate)
    {
        UpdateTagUsageCounts(challengeToCreate);
        UpdateTagNoveltyValues();
      //  PrintTagsNovelty();         // delete, debug only
     //   PrintNoveltyCounts();       // delete, debug only
    }

    private void UpdateTagUsageCounts(PossibleChallengeData challengeToCreate)
    {
        foreach (TypeOfEnemy enemy in challengeToCreate.GetTypeOfEnemies())
        {
            tagUsageCount[enemy.GetEnemyTypeTag()]++;
            totalNumberOfTagsGenerated++;
        }
    }

    private void UpdateTagNoveltyValues()
    {
        int noveltyForTag = 0;
        Dictionary<string, int> medians = new Dictionary<string, int>();
        foreach (string enemy in tagUsageCount.Keys)
        {
            noveltyForTag = tagUsageCount[enemy];
            CompareMedianWithMaxAndMinMedians(noveltyForTag);
        }
        foreach (string enemy in tagUsageCount.Keys)
        {
            tagNoveltyValues[enemy] = (int)(Mathf.InverseLerp(minimumNoveltyMedian,
                                                                maximumNoveltyMedian,
                                                                tagUsageCount[enemy]) * 100);
            //           print("average for " + enemy + " " + tagsPerformanceMedian[enemy]);

        }
        minimumNoveltyMedian = 0;
        maximumNoveltyMedian = 100;
        /*foreach (string tag in tagUsageCount.Keys) // tagUsageCount has to be used since you can't change the values for the dictionary being iterated over
        {
            tagNoveltyValues[tag] = (int) (100 - (float) tagUsageCount[tag] / totalNumberOfTagsGenerated * 100);
        }*/
    }

    private void CompareMedianWithMaxAndMinMedians(int medianPerformance)
    {
        if (medianPerformance < maximumNoveltyMedian)
        {
            maximumNoveltyMedian = medianPerformance;
        }
        if (medianPerformance > minimumNoveltyMedian)
        {
            minimumNoveltyMedian = medianPerformance;
        }
    }

    private void InitializeTagsNoveltyAndUsageCount()
    {
        tagNoveltyValues = new Dictionary<string, int>();
        tagUsageCount = new Dictionary<string, int>();

        var tagManager = GetComponent<TagManager>();
        foreach (string tag in tagManager.GetAllTagsToInitialize())
        {
            tagNoveltyValues.Add(tag, 100);
            tagUsageCount.Add(tag, 0);
        }
    }

    private void PrintTagsNovelty()
    {
        foreach (string tag in tagNoveltyValues.Keys)
        {
            print("novelty for " + tag + " is " + tagNoveltyValues[tag] + "\t with count: " + tagUsageCount[tag]);
        }
    }

    private void PrintNoveltyCounts()
    {
        foreach (string tag in tagUsageCount.Keys)
        {
            print("times used tag: " + tag + " - " + tagUsageCount[tag]);
        }
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
}
