using System;
using UnityEngine;

public class SwitchController : MonoBehaviour 
{
    public event Action OnSwitchTriggered = delegate { };

    private const string animatorTurnedOn = "turnedOn";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var triggeringGameObject = collision.gameObject;

        if(triggeringGameObject.GetComponent<FlyingItem>())
        {
            OnSwitchTriggered();
            ChangeTriggerState();
        }
    }

    private void ChangeTriggerState()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Animator>().SetBool(animatorTurnedOn, true);
    }

}
