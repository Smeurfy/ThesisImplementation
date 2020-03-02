using UnityEngine;
using TMPro;

public class GetQuestionnaireNumber : MonoBehaviour 
{
    private void Start()
    {
        var gameManager = FindObjectOfType<GameManager>();
        if(gameManager)
        {
           GetComponent<TextMeshProUGUI>().text += GameManager.instance.GetPlayerAssignedNumber().ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
