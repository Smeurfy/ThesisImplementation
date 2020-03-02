using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    private List<string> allTags = new List<string>();
    private bool firstTimeThrough = true;

    internal List<string> GetAllTagsToInitialize()
    {
        if(firstTimeThrough)
        {
            GetTagsForAllEnemyTypes(allTags);
            firstTimeThrough = false;
        }
        return allTags;
    }

    private static void GetTagsForAllEnemyTypes(List<string> allTags)
    {
        foreach (TypeOfEnemy typeOfenemy in EnemyLibrary.instance.GetAllPossibleEnemies())
        {
            allTags.Add(typeOfenemy.GetEnemyTypeTag());
        }
    }
}
