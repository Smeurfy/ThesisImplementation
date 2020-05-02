using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    public GameObject instructions;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CloseWindow(Button btn)
    {
		var controlersWindow = btn.transform.parent;
		controlersWindow.gameObject.SetActive(false);
    }
}
