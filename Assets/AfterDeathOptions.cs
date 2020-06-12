using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AfterDeathOptions : MonoBehaviour 
{
	public static AfterDeathOptions instance;

	[SerializeField]
	public GameObject afterDeathMenu;

	public event Action OnTryAgainNow = delegate { };
	public event Action OnTryAgainLater = delegate { };
	public event Action OnRestartSameRun = delegate { };
	public event Action OnRestartNewRun = delegate { };
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

	public void TryAgainNow()
	{
		if(OnTryAgainNow != null)
		{
			Debug.Log("try again");
			OnTryAgainNow();
		}
			
	}
	public void TryAgainLater()
	{
		if(OnTryAgainNow != null)
		{
			Debug.Log("try again later");
			OnTryAgainLater();
		}
			
	}

	public void RestartSameRun()
	{
		Debug.Log("restart same run");
		OnRestartSameRun();
	}

	public void RestartNewRun()
	{
		Debug.Log("restart new run");
		OnRestartNewRun();
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
