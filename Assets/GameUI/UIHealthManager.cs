using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHealthManager : MonoBehaviour 
{
    private PlayerHealthSystem player;
    private TextMeshProUGUI healthText;

    private void Awake()
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

    public void UpdateHealthText(int remainingHealth)
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
