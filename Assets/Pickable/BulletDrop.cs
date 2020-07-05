using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class BulletDrop : MonoBehaviour 
{
    public static event Action<int> OnPickUpBullets = delegate { };

    [SerializeField] AudioClip pickedBullets;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] private Animator animator;

    private byte amountOfBulletsToAdd;
    private AudioSource audioPlayer;
    private PlayerShoot playerShoot;
    private Collider2D col;

    private const string PICKED_UP = "pickedUp";

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.enabled = false;
        StartCoroutine(EnableCollider());
        audioPlayer = GetComponent<AudioSource>();
        amountOfBulletsToAdd = (byte) UnityEngine.Random.Range(3, 10);
        text.text = amountOfBulletsToAdd.ToString();
        AfterDeathOptions.instance.OnSkip += DestroyBullets;
        AfterDeathOptions.instance.OnTryAgainNow += DestroyBullets;
        AfterDeathOptions.instance.OnTryAgainLater += DestroyBullets;
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerShoot = collision.GetComponentInChildren<PlayerShoot>();
        if (collision.GetComponentInChildren<BulletManager>() && collision.isTrigger)
        {
            collision.GetComponentInChildren<BulletManager>().PickedBullets(amountOfBulletsToAdd);
            collision.GetComponentInChildren<DroppedWeaponBulletCounter>().UpdateBulletCount(amountOfBulletsToAdd);
            FeedbackAndDestroy();
        }
        else if(playerShoot != null)
        {
            if (playerShoot.IsPlayerHoldingThrowable())
            {
                playerShoot.PickedBullets(amountOfBulletsToAdd);
                FeedbackAndDestroy();
            }
        }
        playerShoot = null;
    }

    private void FeedbackAndDestroy()
    {
        text.text = "+" + text.text;
        animator.SetTrigger(PICKED_UP);
        audioPlayer.PlayOneShot(pickedBullets);
        DeactivateObject();
        StartCoroutine(WaitForSoundToBeOverAndDestroy());
    }

    private void DeactivateObject()
    {
        foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
        GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator WaitForSoundToBeOverAndDestroy()
    {
        while(audioPlayer.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public void SetAmountOfBulletsToDrop(byte amountToDrop)
    {
        amountOfBulletsToAdd = amountToDrop;
        text.text = amountOfBulletsToAdd.ToString();
    }

    private void DestroyBullets(){
        if(this){
            Destroy(gameObject);
        }
    }
}
