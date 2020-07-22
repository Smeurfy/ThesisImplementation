using System.Collections;
using UnityEngine;
using Thesis.Enemy;
using System.Collections.Generic;
using System;

public class FlyingItem : MonoBehaviour
{
    [SerializeField] float secondsBeforeFlightStops = 2f;
    [SerializeField] private float secondsAfterHittingToStop = .5f;
    [SerializeField] private float secondsBeforeAbleToPickupAgain = 1f;
    [SerializeField] private float secondsBeforeTurningOnCollider = .07f;


    private Item item;
    private Collider2D itemCollider;
    private bool canDamage = true;
    private Rigidbody2D rb;

    public bool CanDamage() { return canDamage; }

    private void Start()
    {
        itemCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ActivateTrigger());
        StartCoroutine(StopFlying(secondsBeforeFlightStops));
    }

    public void SetFlightDirection(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().velocity = direction.normalized * item.GetThrowableSpeed();
        StartCoroutine(EnableCollider());
    }

    public void SetItem(Item itemToSet)
    {
        item = itemToSet;
        SetSprite();
    }

    public int GetDamage()
    {
        return item.GetThrowableDamage();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var enemyHitted = collision.gameObject.GetComponent<EnemyController>();
        if (enemyHitted)
        {
            enemyHitted.Pushed();
            StartCoroutine(StopFlying(secondsAfterHittingToStop));
        }
    }

    private IEnumerator StopFlying(float secondsBeforeStopping)
    {
        yield return new WaitForSecondsRealtime(secondsBeforeStopping);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        canDamage = false;
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSecondsRealtime(secondsBeforeTurningOnCollider);
        itemCollider.enabled = true;
    }

    private IEnumerator ActivateTrigger()
    {
        yield return new WaitForSecondsRealtime(secondsBeforeAbleToPickupAgain);
        GetComponent<CircleCollider2D>().enabled = true;
    }

    private void SetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = item.GetSprite();
    }
}
