using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveEmail : MonoBehaviour {

	public Button nextBtn;
	public void saveUserEmail(string input){
		SimpleEmailSender.SetEmail(input);
	}	

	public void EnableBtnNext(InputField input){
		saveUserEmail(input.text);
		if(input.text != "")
			nextBtn.interactable = true;
		else
			nextBtn.interactable = false;
	}

	public void StartGame(){
		SceneManager.LoadScene("OrderTierMonsters");
	}
}
