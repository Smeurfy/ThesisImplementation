using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    [SerializeField] private Transform target;
    [SerializeField] private float catchUpSpeed = .2f;
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float distanceToLookAt = 1f;

    private Vector3 cameraOffset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        transform.position = Vector3.SmoothDamp(transform.position, PlaceToLookAt(), ref velocity, catchUpSpeed, maxSpeed);
    }

    private Vector3 PlaceToLookAt()
    {
        if(target)
        {
            Vector3 placeToLook = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - target.position).normalized;
            return placeToLook = target.position + placeToLook * distanceToLookAt + cameraOffset;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
