using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReadModelValuesFromPlayer : MonoBehaviour 
{
    public static ReadModelValuesFromPlayer instance;

    public event Action<int, int> OnValuesSubmitted = delegate { }; 

    [SerializeField] private Dropdown noveltyWeightInPercentagePicker;
    [SerializeField] private Dropdown desiredNoveltyInPercentagePicker;

    private int noveltyWeightInPercentage = -4;
    private int desiredNoveltyInPercentage = -6;

    private void Awake()
    {
        MakeThisObjectSingleton();
        noveltyWeightInPercentagePicker.onValueChanged.AddListener(new UnityAction<int>(SetNoveltyWeight));
        desiredNoveltyInPercentagePicker.onValueChanged.AddListener(new UnityAction<int>(SetDesiredNovelty));
        noveltyWeightInPercentage = int.Parse(noveltyWeightInPercentagePicker.captionText.text);
        desiredNoveltyInPercentage = int.Parse(desiredNoveltyInPercentagePicker.captionText.text);
    }

    private void SetNoveltyWeight(int index)
    {
        noveltyWeightInPercentage = int.Parse(noveltyWeightInPercentagePicker.captionText.text);
    }

    private void SetDesiredNovelty(int index)
    {
        desiredNoveltyInPercentage = int.Parse(desiredNoveltyInPercentagePicker.captionText.text);
    }

    public void ValuesConfirmed()
    {
        print("outgoing " + noveltyWeightInPercentage + " and " + desiredNoveltyInPercentage);
        OnValuesSubmitted(noveltyWeightInPercentage, desiredNoveltyInPercentage);
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
