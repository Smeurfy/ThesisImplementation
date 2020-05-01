using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BulletManagerTest : MonoBehaviour 
{
    [SerializeField] private int availableBullets;
    [SerializeField] private Image bulletImage;

    private Canvas UICanvas;

    private void Awake()
    {
        UICanvas = GetComponentInChildren<Canvas>();
        if(!GetComponent<FlyingItem>())
        {
            availableBullets = GetComponent<WeaponPickUpTest>().GetWeapon().GetBulletAmount();
        }
        StartCoroutine(ShowUI());
        CheckAvailableBullets(availableBullets);
    }

    private void AddBulletsToAvailable(int bulletsToAdd)
    {
        availableBullets += bulletsToAdd;
    }

    public void PickedBullets(int numberOfBulletsPicked)
    {
        SetAvailableBullets(numberOfBulletsPicked + availableBullets);
    }

    private IEnumerator ShowUI()
    {
        yield return new WaitForSecondsRealtime(1);
        UICanvas.enabled = true;
    }

    public bool HasAvailableBullets()
    {
        return availableBullets > 0;
    }

    public void BulletFired()
    {
        availableBullets--;
    }

    public void SetAvailableBullets(int availableBulletsRemaning)
    {
        availableBullets = availableBulletsRemaning;
        CheckAvailableBullets(availableBullets);
    }

    public int AvailableBulletsCount()
    {
        return availableBullets;
    }
    
    private void CheckAvailableBullets(int availableBullets)
    {
       /* if(bulletImage == null)
        {
            bulletImage = GetComponentInChildren<Image>();
        }*/
        if(bulletImage != null)
        {
            bulletImage.color = (availableBullets == 0) ? Color.red : Color.white;
        }
    }

}
