using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AfterDeathOptions : MonoBehaviour 
{
	public static AfterDeathOptions instance;

	[SerializeField]
	public GameObject afterDeathMenu;

	public event Action OnTryAgain = delegate { };
	public event Action OnRestart = delegate { };

	public event Action OnSkip = delegate { };

	private void Awake()
    {
        MakeThisObjectSingleton();
    }

	// Use this for initialization
	void Start () 
	{
		afterDeathMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void TryAgain()
	{
		if(OnTryAgain != null)
		{
			Debug.Log("try again");
			OnTryAgain();
		}
			
	}

	public void Restart()
	{
		Debug.Log("restart");
		OnRestart();
	}

	public void Skip()
	{
		Debug.Log("skip");
		OnSkip();
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
