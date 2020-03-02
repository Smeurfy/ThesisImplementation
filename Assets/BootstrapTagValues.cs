using System.Collections.Generic;
using UnityEngine;

public class BootstrapTagValues : MonoBehaviour
{
    public static BootstrapTagValues instance;

    private static Dictionary<string, int> bootstrapValueForTag = new Dictionary<string, int>();
    private static bool firstTime = true;

    [Header("Vanilla")]
    [SerializeField] private int bigZombie;
    [SerializeField] private int chort;
    [SerializeField] private int imp;
    [SerializeField] private int maskedOrc;
    [SerializeField] private int muddy;
    [SerializeField] private int orc;
    [SerializeField] private int shaman;
    [SerializeField] private int skelet;
    [SerializeField] private int swampy;
    [SerializeField] private int iceZombie;

    [Header("Hp mutated")]
    [SerializeField] private int bigZombieHP;
    [SerializeField] private int chortHP;
    [SerializeField] private int impHP;
    [SerializeField] private int maskedOrcHP;
    [SerializeField] private int muddyHP;
    [SerializeField] private int orcHP;
    [SerializeField] private int shamanHP;
    [SerializeField] private int skeletHP;
    [SerializeField] private int swampyHP;
    [SerializeField] private int iceZombieHP;

    [Header("grenade on death mutated")]
    [SerializeField] private int bigZombieGR;
    [SerializeField] private int chortGR;
    [SerializeField] private int impGR;
    [SerializeField] private int maskedOrcGR;
    [SerializeField] private int muddyGR;
    [SerializeField] private int orcGR;
    [SerializeField] private int shamanGR;
    [SerializeField] private int skeletGR;
    [SerializeField] private int swampyGR;
    [SerializeField] private int iceZombieGR;

    private void Awake()
    {
        MakeThisObjectSingleton();
        if(firstTime)
        { 
        // vanilla enemies
            bootstrapValueForTag.Add("BigZombie", bigZombie);
            bootstrapValueForTag.Add("Chort", chort);
            bootstrapValueForTag.Add("Imp", imp);
            bootstrapValueForTag.Add("MaskedOrc", maskedOrc);
            bootstrapValueForTag.Add("Muddy", muddy);
            bootstrapValueForTag.Add("Orc", orc);
            bootstrapValueForTag.Add("Shaman", shaman);
            bootstrapValueForTag.Add("Skelet", skelet);
            bootstrapValueForTag.Add("Swampy", swampy);
            bootstrapValueForTag.Add("IceZombie", iceZombie);

            // hp mutated
            bootstrapValueForTag.Add("BigZombie hp mutated", bigZombieHP);
            bootstrapValueForTag.Add("Chort hp mutated", chortHP);
            bootstrapValueForTag.Add("Imp hp mutated", impHP);
            bootstrapValueForTag.Add("MaskedOrc hp mutated", maskedOrcHP);
            bootstrapValueForTag.Add("Muddy hp mutated", muddyHP);
            bootstrapValueForTag.Add("Orc hp mutated", orcHP);
            bootstrapValueForTag.Add("Shaman hp mutated", shamanHP);
            bootstrapValueForTag.Add("Skelet hp mutated", skeletHP);
            bootstrapValueForTag.Add("Swampy hp mutated", swampyHP);
            bootstrapValueForTag.Add("IceZombie hp mutated", iceZombieHP);
        
            // grenade mutated
            bootstrapValueForTag.Add("BigZombie grenadeOnDeath mutated", bigZombieGR);
            bootstrapValueForTag.Add("Chort grenadeOnDeath mutated", chortGR);
            bootstrapValueForTag.Add("Imp grenadeOnDeath mutated", impGR);
            bootstrapValueForTag.Add("MaskedOrc grenadeOnDeath mutated", maskedOrcGR);
            bootstrapValueForTag.Add("Muddy grenadeOnDeath mutated", muddyGR);
            bootstrapValueForTag.Add("Orc grenadeOnDeath mutated", orcGR);
            bootstrapValueForTag.Add("Shaman grenadeOnDeath mutated", shamanGR);
            bootstrapValueForTag.Add("Skelet grenadeOnDeath mutated", skeletGR);
            bootstrapValueForTag.Add("Swampy grenadeOnDeath mutated", swampyGR);
            bootstrapValueForTag.Add("IceZombie grenadeOnDeath mutated", iceZombieGR);

            firstTime = false;
        }
    }

    public static int GetPerformanceForTag(string tag)
    {
        return bootstrapValueForTag[tag];
    }

    private void MakeThisObjectSingleton()
    {
        DontDestroyOnLoad(gameObject);
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
