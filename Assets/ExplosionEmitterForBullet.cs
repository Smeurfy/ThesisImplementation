using System;
using System.Collections;
using UnityEngine;

public class ExplosionEmitterForBullet : MonoBehaviour 
{
    public static event Action OnBulletExplosion;

    private ParticleSystem explosionPS;
    private ParticleSystem trailPS;
    private AudioSource audioSource;

    private void Awake()
    {
        explosionPS = GetComponentInParent<ParticleSystem>();
        trailPS = GetComponent<ParticleSystem>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void ActivateEmitter()
    {
        OnBulletExplosion();
        Destroy(trailPS);
        explosionPS.Play();
        StartCoroutine(DestroyAfterEmitterStops());
    }

    private IEnumerator DestroyAfterEmitterStops()
    {
        //while(explosionPS.isPlaying || audioSource.isPlaying)
        while(audioSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(transform.parent.gameObject);
    }
}
