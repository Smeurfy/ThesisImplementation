using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBulletUI : MonoBehaviour 
{
    [SerializeField] private PlayerShoot playerShoot;

    private TextMeshProUGUI numberOfBullets;
    private Image bulletImage;
    private static bool firstTime = true;

    private void Start()
    {
        numberOfBullets = GetComponentInChildren<TextMeshProUGUI>();
        bulletImage = GetComponentInChildren<Image>();
        if(firstTime)
        {
            firstTime = false;
            SubscribeToWeaponHeldState();
        }
        DisableBulletUI();
        PlayerShoot.OnBulletPickedUp += UpdateBulletsUI;
    }

    private void SubscribeToWeaponHeldState()
    {
        playerShoot.OnItemPickup += EnableBulletUI;
        playerShoot.OnItemThrow += DisableBulletUI;
        PlayerShoot.OnBulletFired += UpdateBulletsUI;
    }

    private void UpdateBulletsUI(int availableBullets)
    {
        UpdateNumberOfBulletsText(availableBullets);
        CheckAvailableBullets(availableBullets);
    }

    private void WeaponIsEmpty()
    {
        bulletImage.color = Color.red;
    }

    private void UpdateNumberOfBulletsText(int availableBullets)
    {
        numberOfBullets.text = availableBullets.ToString();
    }
    
    private void DisableBulletUI()
    {
        foreach (RectTransform child in transform.GetComponentInChildren<RectTransform>())
        {
            child.gameObject.SetActive(false);
        }
    }

    private void EnableBulletUI(int availableBullets)
    {
        foreach (RectTransform child in transform.GetComponentInChildren<RectTransform>())
        {
            child.gameObject.SetActive(true);
        }
        UpdateBulletsUI(availableBullets);
    }

    private void CheckAvailableBullets(int availableBullets)
    {
        bulletImage.color = (availableBullets == 0) ? Color.red : Color.white;
    }
}
