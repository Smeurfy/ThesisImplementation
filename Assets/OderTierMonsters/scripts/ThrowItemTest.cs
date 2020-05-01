using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowItemTest : MonoBehaviour 
{
    public event Action OnPlayerThrow = delegate { };

    private bool isHoldingItem;
    private bool isGamePaused = false;
    private PlayerShootTest playerShoot;
    private AudioSource audioSource;
    private AudioClipHolder soundFXHolder;

    private const string throwButton = "Throw Weapon";

    private void Start()
    {
        GetReferences();
        GetComponentInChildren<PlayerShootTest>().OnItemPickup += HoldingItem;
        isHoldingItem = playerShoot.IsPlayerHoldingThrowable();
    }
    
    private void Update()
    {
        if (ThrowItemButtonPressed() && PlayerMovement.characterCanReceiveInput && !isGamePaused)
        {
            AttemptToThrowItem();
        }
    }

    private void DropWeaponAfterGameOver(Scene loadedScene)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            PauseMenuManager.instance.OnGameIsPaused -= GameIsPaused;
            DropWeaponAfterGameOver();
        }
    }

    private void DropWeaponAfterGameOver()
    {
        isHoldingItem = false;
        OnPlayerThrow();
    }

    private void AttemptToThrowItem(bool isFromGameOver = false)
    {
        if (isHoldingItem)
        {
            audioSource.PlayOneShot(soundFXHolder.GetThrowedSound());
            Throw(playerShoot.GetWeaponBeingHeld());
            OnPlayerThrow();
        }
        isHoldingItem = false;
    }

    private bool ThrowItemButtonPressed()
    {
        return Input.GetButtonDown(throwButton);
    }

    public void Throw(Item itemToThrow)
    {
        var flyingItem = Instantiate(itemToThrow.GetFlyingItemPrefab(), transform.position, Quaternion.identity);
        flyingItem.GetComponent<FlyingItem>().SetItem(itemToThrow);
        flyingItem.GetComponent<FlyingItem>().SetFlightDirection(CalculateFlightDirection());
        flyingItem.GetComponent<WeaponPickUpTest>().SetWeapon((Weapon)itemToThrow);
        int remainingBulletsInWeapon = GetComponentInChildren<PlayerShootTest>().GetRemainingBullets();
        flyingItem.GetComponent<BulletManagerTest>().SetAvailableBullets(remainingBulletsInWeapon);
    }
    
    private Vector2 CalculateFlightDirection()
    {
        Vector2 movementDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - FindObjectOfType<PlayerMovement>().transform.position;
        movementDirection.Normalize();
        return movementDirection;
    }

    private void HoldingItem(int availableBullets)
    {
        audioSource.PlayOneShot(soundFXHolder.GetPickUpSound());
        isHoldingItem = true;
    }
    
    private void GetReferences()
    {
        playerShoot = GetComponentInChildren<PlayerShootTest>();
        audioSource = GetComponent<AudioSource>();
        soundFXHolder = GetComponent<AudioClipHolder>();
    }

    private void GameIsPaused(bool isGamePaused)
    {
        this.isGamePaused = isGamePaused;
    }
}
