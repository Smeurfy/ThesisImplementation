using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    public static bool characterCanReceiveInput = true;

    [SerializeField] float walkingSpeed;
    [SerializeField] ParticleSystem walkingPS;

    private Rigidbody2D rb;
    private Animator animator;

    private const string animatorXMovementAxis = "Walk Sideways";
    private const string animatorYMovementAxis = "Walk Vertically";
    private const string animatorMovementSpeed = "movementSpeed";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        walkingPS = Instantiate(walkingPS, gameObject.transform);
    }

    void FixedUpdate()
    {
        if(characterCanReceiveInput)
        {
            rb.velocity = ProcessMovementInput() * walkingSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if(rb.velocity == Vector2.zero && characterCanReceiveInput) // I have no idea why this works
        {
            PlayWalkingParticleSystem();
        }
    }

    public static void CharacterCanReceiveInput(bool canUpdate)
    {
        characterCanReceiveInput = canUpdate;
    }

    private void PlayWalkingParticleSystem()
    {
        walkingPS.Play();
    }

    private Vector3 ProcessMovementInput()
    {
        float xInput = Input.GetAxisRaw(animatorXMovementAxis), yInput = Input.GetAxisRaw(animatorYMovementAxis);
        Vector3 movementDirection = new Vector3(xInput, yInput, 0).normalized;
        UpdateMovementSpeedInAnimator(movementDirection);

        return movementDirection;
    }

    private void UpdateMovementSpeedInAnimator(Vector3 movementDirection)
    {
        animator.SetFloat(animatorMovementSpeed, movementDirection.magnitude);
    }
}
