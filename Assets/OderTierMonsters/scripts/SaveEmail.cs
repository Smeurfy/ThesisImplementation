using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net.Mail;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class SaveEmail : MonoBehaviour
{
	public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public Button nextBtn;

    public void EnableBtnNext(InputField input)
    {  
        if(IsEmail(input.text)){
			nextBtn.interactable = true;
		}
		else{
			nextBtn.interactable = false;
		}   
    }

	    public void saveUserEmail(string input)
    {
        SimpleEmailSender.SetEmail(input);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("OrderTierMonsters");
    }

	public static bool IsEmail(string email)
        {
            if (email != null) 
				return Regex.IsMatch(email, MatchEmailPattern);
            else 
				return false;
        }
}
