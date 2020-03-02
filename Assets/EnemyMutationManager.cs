using System.Collections.Generic;
using UnityEngine;

public struct MutatedEnemy
{
    private GameObject enemyPrefab;
    private TypeOfEnemy mutantData;

    public MutatedEnemy(GameObject enemyPrefab, TypeOfEnemy mutantData)
    {
        this.enemyPrefab = enemyPrefab;
        this.mutantData = mutantData;
    }

    public GameObject GetEnemyPrefab() { return enemyPrefab; }
    public TypeOfEnemy GetMutantData() { return mutantData; }
}

public class EnemyMutationManager : MonoBehaviour 
{
    public static EnemyMutationManager instance;

    [SerializeField] private List<Mutation> possibleMutations;

    private List<string> allPossibleMutatedEnemies;

    public int GetAmountOfPossibleMutations() { return possibleMutations.Count; }

    private void Awake()
    {
        MakeThisObjectSingleton();
        allPossibleMutatedEnemies = new List<string>();
    }

    public Mutation AssignMutation(StatToMutate stateToMutate)
    {
        foreach (Mutation mutation in possibleMutations)
        {
            if (Equals(mutation.GetMutatedStat(), stateToMutate))
            {
                return mutation;
            }
        }
        Debug.LogError("Mutation doesn't exist");
        return ScriptableObject.CreateInstance<Mutation>();
    }

    internal void GenerateAllPossibleMutations(List<TypeOfEnemy> allPossibleEnemies)
    {
        foreach(TypeOfEnemy typeOfEnemy in allPossibleEnemies)
        {
            foreach (Mutation mutation in possibleMutations)
            {
                string newMutatedEnemyString = typeOfEnemy.GetEnemyTypeTag() + mutation.GetMutationString();
                allPossibleMutatedEnemies.Add(newMutatedEnemyString);
            }
        }
        //PrintAllPossibleMutatedEnemies();
    }
    
   public MutatedEnemy GetRandomMutatedEnemy()
    {
        string mutatedEnemyToBeUsed = allPossibleMutatedEnemies[Random.Range(0, allPossibleMutatedEnemies.Count)];
        allPossibleMutatedEnemies.Remove(mutatedEnemyToBeUsed);
        return ConvertStringToMutatedEnemy(mutatedEnemyToBeUsed);     
    }

    private MutatedEnemy ConvertStringToMutatedEnemy(string mutatedEnemyToBeGenerated)
    {
        string[] mutantStrings = mutatedEnemyToBeGenerated.Split(null);
        GameObject enemyPrefab = GetEnemyPrefab(mutantStrings);
        TypeOfEnemy mutantData = GetMutantData(mutantStrings, enemyPrefab);

        return new MutatedEnemy(enemyPrefab, mutantData);
    }

    private GameObject GetEnemyPrefab(string[] mutantStrings)
    {
        string enemyPrefabString = mutantStrings[0];
        GameObject enemyPrefab = EnemyLibrary.instance.GetEnemyTypePrefab(enemyPrefabString);
        return enemyPrefab;
    }

    private TypeOfEnemy GetMutantData(string[] mutantStrings, GameObject enemyPrefab)
    {
        string mutantDataString = " " + mutantStrings[1];
        TypeOfEnemy mutantData = GetMutationByName(mutantDataString, enemyPrefab);
        return mutantData;
    }

    private TypeOfEnemy GetMutationByName(string mutationString, GameObject enemyToMutate)
    {
        TypeOfEnemy enemyToMutateInfo = enemyToMutate.GetComponent<EnemyData>().GetTypeOfEnemy();
        var mutatedEnemyInfo = Instantiate(enemyToMutateInfo);
        
        foreach(Mutation mutation in possibleMutations)
        {
            if(string.Equals(mutation.GetMutatedStatString(), mutationString))
            {
                mutatedEnemyInfo.MutateStat(mutation.GetMutatedStat());
            }
        }
        return mutatedEnemyInfo;        
    }
    
    private void PrintAllPossibleMutatedEnemies()
    {
        foreach (string mutatedPossibility in allPossibleMutatedEnemies)
        {
            print(mutatedPossibility);
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
