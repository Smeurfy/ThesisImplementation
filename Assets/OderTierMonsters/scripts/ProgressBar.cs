using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ProgressBar : MonoBehaviour
{

    public Slider slider;
    public ParticleSystem particleSys;
	float fillSpeed = 0.1f;
	float targetProgress = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if(slider.value < targetProgress){
			slider.value += fillSpeed * Time.deltaTime;
			if(!particleSys.isPlaying){
				particleSys.Play();
			}
		}
		else{
			particleSys.Stop();
            slider.value = (float)Math.Round(slider.value, 1);
		}
    }

    public void IncreaseProgress(float newProgress)
    {
        targetProgress = (float)Math.Round(slider.value, 1) + newProgress;
    }
}
