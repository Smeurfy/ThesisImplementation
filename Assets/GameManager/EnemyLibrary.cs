using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyLibrary : MonoBehaviour 
{
    public static EnemyLibrary instance;

    [SerializeField] private GameObject mutatedEnemiesJail;
    [SerializeField] private int desiredAmountOfMutatedEnemies;
    [SerializeField] private List<GameObject> enemyTypePrefabs;

    private List<TypeOfEnemy> possibleEnemies;
    private Dictionary<TypeOfEnemy, GameObject> typeOfEnemyToPrefab;
    private static EnemyMutationManager mutationManager;
    private static bool firstTime = true;

    #region getters
    public List<TypeOfEnemy> GetAllPossibleEnemies() { return possibleEnemies; }
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this);
        MakeThisObjectSingleton();
        CheckIfEnemyTypePrefabsIsEmpty();
        if (firstTime)
        {
            InitializeVariablesAndGetReferences();
            if(AreMutationAmountsPossible())
            {
                mutationManager.GenerateAllPossibleMutations(possibleEnemies);
                firstTime = false;
                GenerateMutatedEnemies();
            }
            EnemyLibraryData.SetPossibleEnemies(possibleEnemies);
            EnemyLibraryData.SetTypeOfEnemyToPrefab(typeOfEnemyToPrefab);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void GenerateMutatedEnemies()
    {
        List<GameObject> mutatedPrefabs = new List<GameObject>();
        
        for (int i = 0; i < desiredAmountOfMutatedEnemies; i++)
        {
            GameObject mutatedEnemy = GenerateMutantEnemy();
            mutatedPrefabs.Add(mutatedEnemy);
        }
        if (mutatedPrefabs.Count > 0)
        {
            AddMutatedPrefabsToEnemyPool(mutatedPrefabs);
        }
    }

    private GameObject GenerateMutantEnemy()
    {
        MutatedEnemy mutatedEnemyData = mutationManager.GetRandomMutatedEnemy();

        GameObject enemyToMutate = mutatedEnemyData.GetEnemyPrefab();
        TypeOfEnemy mutatedEnemyInfo = mutatedEnemyData.GetMutantData();
        GameObject mutatedEnemy = GenerateMutatedEnemy(new MutatedEnemy(enemyToMutate, mutatedEnemyInfo));
        EnemyLibraryData.SetMutant(new MutatedEnemy(enemyToMutate, mutatedEnemyInfo));
        return mutatedEnemy;
    }
        
    private GameObject GenerateMutatedEnemy(MutatedEnemy enemyToMutate)
    {
        GameObject mutatedEnemy = Instantiate(enemyToMutate.GetEnemyPrefab(), mutatedEnemiesJail.transform);
        var mutatedEnemyData = mutatedEnemy.GetComponent<EnemyData>();
        mutatedEnemyData.SetEnemyType(enemyToMutate.GetMutantData());
        var spriteRenderer = mutatedEnemy.GetComponent<SpriteRenderer>();
        if(spriteRenderer)
        {
            mutatedEnemy.GetComponent<SpriteRenderer>().color = mutatedEnemyData.GetMutationColor();
        }
        else
        {
            mutatedEnemy.GetComponentInChildren<SpriteRenderer>().color = mutatedEnemyData.GetMutationColor();
        }
        return mutatedEnemy;
    }

    public GameObject GetEnemyTypePrefab(TypeOfEnemy toe)
    {
        return typeOfEnemyToPrefab[toe];
    }

    public GameObject GetEnemyTypePrefab(string typeOfEnemyString)
    {
        foreach(TypeOfEnemy toe in typeOfEnemyToPrefab.Keys)
        {
            if (string.Equals(toe.GetEnemyTypeTag(), typeOfEnemyString))
            {
                return typeOfEnemyToPrefab[toe];
            }
        }
        Debug.LogError("EnemyLibrary: GetEnemyTypePrefab by string: can't find enemy");
        return new GameObject();
    }

    private void InitializePossibleEnemies()
    {
        List<GameObject> enemiesToRemoveFromPrefab;
        enemiesToRemoveFromPrefab = new List<GameObject>();
        foreach(GameObject enemyPrefab in enemyTypePrefabs)
        {
            if(enemyPrefab != null)
            {
                TypeOfEnemy enemyType = enemyPrefab.GetComponent<EnemyData>().GetTypeOfEnemy();
                typeOfEnemyToPrefab.Add(enemyType, enemyPrefab);
                possibleEnemies.Add(enemyType);
            }
            else
            {
                enemiesToRemoveFromPrefab.Add(enemyPrefab);
            }
        }
        foreach(GameObject emptyEnemyToRemove in enemiesToRemoveFromPrefab)
        {
            enemyTypePrefabs.Remove(emptyEnemyToRemove);
        }
    }
    
    private void AddMutatedPrefabsToEnemyPool(List<GameObject> mutatedPrefabs)
    {
        foreach (GameObject mutatedEnemy in mutatedPrefabs)
        {
            enemyTypePrefabs.Add(mutatedEnemy);
            typeOfEnemyToPrefab.Add(mutatedEnemy.GetComponent<EnemyData>().GetTypeOfEnemy(), mutatedEnemy);
            possibleEnemies.Add(mutatedEnemy.GetComponent<EnemyData>().GetTypeOfEnemy());
        }
    }

    public TypeOfEnemy GetRandomEnemy()
    {
        return possibleEnemies[Random.Range(0, possibleEnemies.Count)];
    }

    private void CheckIfEnemyTypePrefabsIsEmpty()
    {
        if (enemyTypePrefabs.Capacity == 0)
        {
            Debug.LogError("Assign enemy type prefabs in object GameManager, in EnemyLibrary.cs");
        }
    }
    
    private void InitializeVariablesAndGetReferences()
    {
        possibleEnemies = new List<TypeOfEnemy>();
        typeOfEnemyToPrefab = new Dictionary<TypeOfEnemy, GameObject>();
        mutationManager = GetComponent<EnemyMutationManager>();
        InitializePossibleEnemies();
    }

    private bool AreMutationAmountsPossible()
    {
        if (desiredAmountOfMutatedEnemies > enemyTypePrefabs.Count * mutationManager.GetAmountOfPossibleMutations())
        {
            Debug.LogError("Can't create this many different mutations with such few enemies and mutations");
            return false;
        }
        return true;
    }

    private void SceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            mutatedEnemiesJail = GameObject.FindGameObjectWithTag("Jail");
            
            possibleEnemies = EnemyLibraryData.GetPossibleEnemies();
            typeOfEnemyToPrefab = EnemyLibraryData.GetTypeOfEnemyToPrefab(mutatedEnemiesJail);
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
    }
}

