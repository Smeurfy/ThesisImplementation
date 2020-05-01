using UnityEngine;
using TMPro;

public class DroppedWeaponBulletCounter : MonoBehaviour 
{
    TextMeshProUGUI bulletCount;

    private void Start()
    {
        bulletCount = GetComponent<TextMeshProUGUI>();
        bulletCount.text = GetComponentInParent<BulletManagerTest>().AvailableBulletsCount().ToString();
    }

    public void UpdateBulletCount(int bulletsToAdd)
    {
        bulletCount.text = (int.Parse(bulletCount.text) + bulletsToAdd).ToString();
    }
}
