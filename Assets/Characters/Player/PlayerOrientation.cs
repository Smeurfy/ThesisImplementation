using UnityEngine;

public class PlayerOrientation : MonoBehaviour 
{
    private Animator animator;

    private const string animatorOrientation = "orientation";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        UpdatePlayerOrientation();
    }
    
    private void UpdatePlayerOrientation()
    {
        float CursorXPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        float playerOrientation = CursorXPosition - transform.position.x;
        playerOrientation = playerOrientation > 0 ? 1 : 0;
        UpdateAnimator(playerOrientation);
    }

    private void UpdateAnimator(float orientation)
    {
        animator.SetFloat(animatorOrientation, orientation);
    }
}
