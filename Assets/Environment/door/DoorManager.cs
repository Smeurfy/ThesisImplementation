using System;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public enum DoorsPosition { right, left, top, down }
    public event Action OnPlayerEnteredRoom  = delegate { };

    [SerializeField] private DoorsPosition doorPositionInRoom = DoorsPosition.right;
    [SerializeField] private AudioClip openDoor;
    [SerializeField] private AudioClip closeDoor;

    private Animator animator;
    private AudioSource audioSource;

    private const float triggerOffset = 2.4f;
    private const string animatorBoolIsOpened = "isOpen";

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        GetComponentInParent<RoomManager>().RoomCleared += OpenDoor;
        StartTriggerToCloseDoorAfterPlayerEnteredTheRoom();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var triggerObject = other.gameObject;
        if (triggerObject.GetComponent<PlayerMovement>())
        {
            OnPlayerEnteredRoom();
            UpdateCameraToLookAtNewRoom();
            CloseDoor();
            DeletePlayerEnteredRoomTrigger();
        }
    }

    private void UpdateCameraToLookAtNewRoom()
    {
        if (CameraLookAtRoom.instance)
        {
            CameraLookAtRoom.instance.SetNewCameraPosition();
        }
    }

    private void DeletePlayerEnteredRoomTrigger()
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            if(collider.isTrigger)
            {
                Destroy(collider);
            }
        }
    }

    private void OpenDoor()
    {
        ChangeDoorIsOpen(true);
        audioSource.PlayOneShot(openDoor);
        animator.SetBool(animatorBoolIsOpened, true);
    }
    
    private void CloseDoor()
    {
        ChangeDoorIsOpen(false);
        audioSource.PlayOneShot(closeDoor);
        animator.SetBool(animatorBoolIsOpened, false);
    }

    private void ChangeDoorIsOpen(bool newState)
    {
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (!col.isTrigger)
            {
                GetComponent<Collider2D>().enabled = !newState;
            }
        }
    }

    private void StartTriggerToCloseDoorAfterPlayerEnteredTheRoom()
    {
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (col.isTrigger)
            {
                switch (doorPositionInRoom)
                {
                    case DoorsPosition.right:
                        col.offset = new Vector2(triggerOffset, 0);
                        break;
                    case DoorsPosition.left:
                        col.offset = new Vector2(-triggerOffset, 0);
                        break;
                    case DoorsPosition.top:
                        col.offset = new Vector2(0, triggerOffset);
                        break;
                    case DoorsPosition.down:
                        col.offset = new Vector2(0, -triggerOffset);
                        break;
                }
            }
        }
    }
}
