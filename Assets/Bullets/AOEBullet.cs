using System.Collections;
using UnityEngine;

public class AOEBullet : TypeOfBullet
{
    [SerializeField] private float timeToExplode = 2f;
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private float impactSpeed = 15f;
    [SerializeField] private SpriteRenderer explosionRepresentation;
    [SerializeField] private AudioClip explosionSound;
    
    private Animator animator;
    private AudioSource audioSource;

    private const string animatorExplosion = "explosion";
    private const string animatorBlink = "blink";

    private void Start()
    {
        StartCoroutine(BlinkBeforeExplosion());
        StartCoroutine(ExplodeAfterTimer());
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        isDamageImmediate = false;
    }

    private IEnumerator BlinkBeforeExplosion()
    {
        yield return new WaitForSecondsRealtime(timeToExplode - 1.0f);
        animator.SetTrigger(animatorBlink);
        explosionRepresentation.enabled = true;

    }

    private IEnumerator ExplodeAfterTimer()
    {
        yield return new WaitForSecondsRealtime(timeToExplode);
        explosionRepresentation.enabled = false;
        GetComponent<ParticleSystem>().Play();
        audioSource.PlayOneShot(explosionSound);
        animator.SetTrigger(animatorExplosion);
        DamageCharactersInRange();
    }
    
    private void DamageCharactersInRange()
    {
        Collider2D[] charactersInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach(Collider2D character in charactersInRange)
        {
            var characterHealth = character.GetComponent<HealthSystem>();
            if (characterHealth)
            {
                characterHealth.TakeDamage(damage);
            }
            else if(character.GetComponentInParent<HealthSystem>())
            {
                character.GetComponentInParent<HealthSystem>().TakeDamage(damage);
            }
            else if(character.GetComponent<WeaponPickup>() && character != this)
            {
                try
                {
                    character.GetComponent<BoxCollider2D>().enabled = true;
                    character.GetComponent<Rigidbody2D>().velocity = (character.transform.position - transform.position).normalized * impactSpeed;
                }
                catch(MissingComponentException) { Debug.LogWarning("Character in explosion radius doesn't have Rigidbody2D"); }
            }
        }
    }

    public override void SetTarget(Vector2 target)
    {
        rb.velocity = (target - (Vector2) transform.position).normalized * speed;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<HealthSystem>())
        {
            rb.velocity = Vector2.zero;
            
        }
    }
}
