using System.Collections.Generic;
using UnityEngine;

public static class EnemyLibraryData  
{
    private static List<TypeOfEnemy> possibleEnemies = new List<TypeOfEnemy>();
    private static Dictionary<TypeOfEnemy, GameObject> typeOfEnemyToPrefab = new Dictionary<TypeOfEnemy, GameObject>();
    public static List<MutatedEnemy> mutants = new List<MutatedEnemy>();

    public static void SetPossibleEnemies(List<TypeOfEnemy> possibleEnemiesToSet)
    {
        possibleEnemies = possibleEnemiesToSet;
    }

    public static void SetTypeOfEnemyToPrefab (Dictionary<TypeOfEnemy, GameObject> typeOfEnemyToPrefabToSet)
    {
        typeOfEnemyToPrefab = typeOfEnemyToPrefabToSet;
    }

    private static void InstantiateMutant(GameObject mutantJail, int index)
    {
        GameObject mutatedEnemy = UnityEngine.Object.Instantiate(mutants[index].GetEnemyPrefab(), mutantJail.transform);
        var mutatedEnemyData = mutatedEnemy.GetComponent<EnemyData>();
        mutatedEnemyData.SetEnemyType(mutants[index].GetMutantData());
        var sr = mutatedEnemy.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = mutatedEnemyData.GetMutationColor();
        else
            mutatedEnemy.GetComponentInChildren<SpriteRenderer>().color = mutatedEnemyData.GetMutationColor();
        typeOfEnemyToPrefab.Add(mutants[index].GetMutantData(), mutatedEnemy);
    }

    public static void SetMutant(MutatedEnemy mutantToSet)
    {
        mutants.Add(mutantToSet);
    }

    internal static List<TypeOfEnemy> GetPossibleEnemies()
    {
        return possibleEnemies;
    }

    internal static Dictionary<TypeOfEnemy, GameObject> GetTypeOfEnemyToPrefab(GameObject mutantJail)
    {
        List<TypeOfEnemy> enemiesToDelete = new List<TypeOfEnemy>();

        foreach(TypeOfEnemy toe in typeOfEnemyToPrefab.Keys)    // store enemies with no gameobjects
        {
            if(typeOfEnemyToPrefab[toe] == null)
            {
                enemiesToDelete.Add(toe);
            }
        }
        foreach(TypeOfEnemy toe in enemiesToDelete)     // delete said enemies
        {
            typeOfEnemyToPrefab.Remove(toe);
        }
        
        for(int i = 0; i < mutants.Count; i++)  // add mutations and their GO back to the dictionary
        {
            InstantiateMutant(mutantJail, i);
        }

        return typeOfEnemyToPrefab;
    }
}
