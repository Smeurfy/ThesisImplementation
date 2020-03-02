using UnityEngine;

public enum StatToMutate { onDeath, hp};

[CreateAssetMenu(fileName = "new mutation", menuName = "Mutation")]
public class Mutation : ScriptableObject 
{
    [SerializeField] private StatToMutate mutatedStat;
    [SerializeField] private int statAddition;
    [SerializeField] private Color mutationColor;

    private string mutatedStatString;

    internal StatToMutate GetMutatedStat() { return mutatedStat; }

    internal int GetStatAddition() { return statAddition; }

    internal string GetMutationString() { return mutatedStatString; }

    internal Color GetMutationColor() { return mutationColor; }

    internal string GetMutatedStatString() { return mutatedStatString; }
    
    private void OnEnable()
    {
        SetStatString();
    }

    // IMPORTANT: THIS STRING NEEDS TO BE 1 'WORD' ONLY
    private void SetStatString()
    {
        switch (mutatedStat)
        {
            case (StatToMutate.onDeath):
                mutatedStatString = " grenadeOnDeath";
                break;
            case (StatToMutate.hp):
                mutatedStatString = " hp";
                break;
        }
    }

}
