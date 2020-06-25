using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPoints : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(gameObject, 1);
		transform.localPosition += new Vector3(0f, 0.5f, 0f);
	}
	
}
