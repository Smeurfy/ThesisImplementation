using System.Collections.Generic;
using UnityEngine;

public class PressSwitchChallenge : RoomChallenge
{
    private List<SwitchController> switches;
    private int switchesAlreadyActivated = 0;
    private int totalNumberOfSwitches;

    private void Update()
    {
        CheckIfAllSwitchesHaveBeenPressed();
    }

    public void InitializeSwitches(GameObject switchesHolder)
    {
        switches = new List<SwitchController>();
        foreach (SwitchController switchi in switchesHolder.transform.GetComponentsInChildren<SwitchController>())
        {
            switches.Add(switchi);
            switchi.OnSwitchTriggered += SwitchWasTriggered;
            totalNumberOfSwitches++;

        }
    }
    
    private void CheckIfAllSwitchesHaveBeenPressed()
    {
        if (switchesAlreadyActivated == totalNumberOfSwitches)
        {
            ChallengeComplete();
            Destroy(this);
        }
    }

    private void SwitchWasTriggered() 
    {
        switchesAlreadyActivated++;
    }
}
