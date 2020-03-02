using UnityEngine;

public class WeaponLookAt : MonoBehaviour
{
    [SerializeField] private float spriteAngleOffset = 0;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        float angleRotation = CalculateAngleBetweenWeaponAndCursor();
        UpdateSpriteDirection(angleRotation);
        UpdateWeaponRotation(angleRotation);
    }

    private void UpdateWeaponRotation(float angleRotation)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, angleRotation + spriteAngleOffset);
    }

    private float CalculateAngleBetweenWeaponAndCursor()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cursorDirection = (cursorPosition - transform.position).normalized;
        float angleRotation = Mathf.Atan2(cursorDirection.y, cursorDirection.x) * Mathf.Rad2Deg;
        return angleRotation;
    }

    private void UpdateSpriteDirection(float newRotation)
    {
        if (newRotation < -90 || newRotation > 90)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }
}
