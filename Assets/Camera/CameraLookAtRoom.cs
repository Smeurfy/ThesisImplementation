using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraLookAtRoom : MonoBehaviour 
{
    public static CameraLookAtRoom instance;

    [SerializeField] private Transform roomToLookAt;
    [SerializeField] private float catchUpSpeed;
    [SerializeField] private float maxSpeed;

    //private Vector3 initialRoomPosition;
    private Vector3 nextRoomsPosition;
    private bool isTimeToChangePosition = false;
    private readonly Vector3 cameraOffset = Vector3.back * 10;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        MakeThisObjectSingleton();
        transform.position = roomToLookAt.position + cameraOffset;
    }

    private void FixedUpdate()
    {
        LerpCameraBetweenRooms();
    }

    private void LerpCameraBetweenRooms()
    {
        if (isTimeToChangePosition && Vector3.Distance(transform.position, nextRoomsPosition) > .1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, nextRoomsPosition, ref velocity, catchUpSpeed, maxSpeed);
        }
        else
        {
            isTimeToChangePosition = false;
        }
    }

    public void NextRoomsPosition(Vector3 nextRoomPos)
    {
        nextRoomsPosition = nextRoomPos + cameraOffset;
    }

    public void SetNewCameraPosition()
    {
        isTimeToChangePosition = true;
    }

    private void MakeThisObjectSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    
}
