﻿using UnityEngine;
using System.Collections;
using System;

public class RegularBullet : TypeOfBullet
{
    private void Start()
    {
        AfterDeathOptions.instance.OnTryAgainNow += DestroyBullet;
        AfterDeathOptions.instance.OnTryAgainLater += DestroyBullet;
        AfterDeathOptions.instance.OnRestartNewRun += DestroyBullet;
        AfterDeathOptions.instance.OnRestartSameRun += DestroyBullet;
        AfterDeathOptions.instance.OnSkip += DestroyBullet;
        RandomColor();
    }

    private void RandomColor()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        sprite.color = color;
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule ma = ps.main;
        ma.startColor = color;
    }


    public override void SetTarget(Vector2 target)
    {
        SetBulletDirection(target);
    }

    protected virtual void SetBulletDirection(Vector2 directionToSet)
    {
        Vector2 movementDirection = directionToSet - (Vector2)transform.position;
        movementDirection.Normalize();
        rb.velocity = movementDirection * speed;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        var objectHitted = collision.gameObject.GetComponent<HealthSystem>();
        if (objectHitted)
        {
            objectHitted.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private void DestroyBullet()
    {
        if (this)
        {
            Destroy(gameObject);
        }
    }
}
