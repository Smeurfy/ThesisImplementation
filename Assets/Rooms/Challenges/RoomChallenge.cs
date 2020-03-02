using System;
using UnityEngine;

public abstract class RoomChallenge : MonoBehaviour
{
    public event Action RoomCleared = delegate { };

    private DoorManager[] roomExits;
    
    public void SetRoomExits(GameObject roomsHolder)
    {
        roomsHolder.transform.GetComponentsInChildren<DoorManager>();
    }

    internal void ChallengeComplete()
    {
        RoomCleared();
    }
}
