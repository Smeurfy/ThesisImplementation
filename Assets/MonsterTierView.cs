using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTierView : MonoBehaviour {

	public Canvas canvas;

	// Update is called once per frame
		void Update(){
		if(Input.GetKeyDown(KeyCode.T)){
			canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
		}
	}
}
