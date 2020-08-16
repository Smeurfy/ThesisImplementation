using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
	Text _text;
    private void Start()
    {
		_text = GetComponentInChildren<Text>();
		StartCoroutine(AnimText());
    }

    private IEnumerator AnimText()
    {
		yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0f, 1f));
		if(_text.text == "Loading...")
			_text.text = "Loading";
		else
			_text.text += ".";
		StartCoroutine(AnimText());
    }
}
