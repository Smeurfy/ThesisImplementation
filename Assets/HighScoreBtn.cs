using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreBtn : MonoBehaviour {

	public void LoadNextScene(){
		SceneManager.LoadScene("ThankYou");
	}
}
