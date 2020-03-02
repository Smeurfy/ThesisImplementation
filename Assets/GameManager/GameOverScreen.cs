using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI performanceText;

    private void Start()
    {
        performanceText.text = PerformanceData.instance.GetMedianPerformanceValue();
    }
}
