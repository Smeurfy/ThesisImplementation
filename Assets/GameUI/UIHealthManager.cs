using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHealthManager : MonoBehaviour 
{
    private PlayerHealthSystem player;
    private TextMeshProUGUI healthText;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerHealthSystem>();
        }
        SceneManager.sceneUnloaded += DereferencePlayerHealth;
        healthText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateHealthText(player.GetCurrentHP());
        player.OnPlayerHealthUpdate += UpdateHealthText;
    }

    private void UpdateHealthText(int remainingHealth)
    {
        healthText.text = remainingHealth.ToString();
    }

    private void DereferencePlayerHealth(Scene loadedScene)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            player.OnPlayerHealthUpdate -= UpdateHealthText;
        }
    }
}
