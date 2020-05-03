using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    public Slider slider;
    public ParticleSystem particleSys;
	float fillSpeed = 0.05f;
	float targetProgress = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		Debug.Log(targetProgress);
		if(slider.value < targetProgress){
			slider.value += fillSpeed * Time.deltaTime;
			if(!particleSys.isPlaying){
				particleSys.Play();
			}
		}
		else{
			particleSys.Stop();
		}
    }

    public void IncreaseProgress(float newProgress)
    {
        targetProgress = slider.value + newProgress;
    }
}
