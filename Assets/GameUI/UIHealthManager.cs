using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIHealthManager : MonoBehaviour 
{
    private PlayerHealthSystem player;
    public GameObject _heart;
    public GameObject _heartPlaceholder;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerHealthSystem>();
        }
        SceneManager.sceneUnloaded += DereferencePlayerHealth;
        UpdateHealthText(player.GetCurrentHP());
        player.OnPlayerHealthUpdate += UpdateHealthText;
    }

    public void UpdateHealthText(int remainingHealth)
    {
        var heartImages = Transform.FindObjectsOfType<Image>();
        foreach (var item in heartImages)
        {
            if(item.name == "Heart(Clone)")
                Destroy(item.gameObject);
        }
        for (int i = 0; i < remainingHealth; i++)
        {
            Instantiate(_heart, _heartPlaceholder.transform.position, Quaternion.identity, _heartPlaceholder.transform);
        }
    }

    private void DereferencePlayerHealth(Scene loadedScene)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            player.OnPlayerHealthUpdate -= UpdateHealthText;
        }
    }
}
