using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;
using System;

public class LoadFromFile : MonoBehaviour
{
    public Canvas canvas;
    public Dropdown dropdown;
    public Text popUp;
    //The list of messages for the Dropdown
    List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
    Dropdown.OptionData m_NewData;
    int selectedFile;
	public GameObject saveBtn;

    void Start()
    {
		saveBtn.GetComponent<WriteOnFile>().OnFileSaved += InitDropDown;
        InitDropDown();
    }

    public void LoadData()
    {
        var dic = canvas.GetComponent<PopulateWithMonsters>().monstersInfo;
        string path = Application.dataPath + "/Resources/" + dropdown.options[selectedFile].text;
		Debug.Log(path);
        string[] fileContent = File.ReadAllLines(path);
		foreach (var item in dic)
		{
			item.Value.Clear();
		}
        foreach (var item in dic)
        {
            foreach (var str in fileContent)
            {
                MonstersInfo obj = JsonUtility.FromJson<MonstersInfo>(str);
                if (obj.monsterName == item.Key)
                {
                    item.Value.Add(obj.tier, obj);
                }
            }
        }
        canvas.GetComponent<PopulateWithMonsters>().monstersInfo = dic;
        canvas.GetComponent<PopulateWithMonsters>().DestroyMonster();
        canvas.GetComponent<PopulateWithMonsters>().UnselectBtn();
        canvas.GetComponent<PopulateWithMonsters>().Populate();
        popUp.gameObject.SetActive(true);
        StartCoroutine(DisablePopUp());
    }

    private void InitDropDown()
    {
        dropdown.ClearOptions();
		m_Messages.Clear();
        string[] a = Directory.GetFiles(Application.dataPath + "/Resources/", "*.json");
        if (a.Length > 0)
        {
            foreach (var item in a)
            {
                //Create a new option for the Dropdown menu
                m_NewData = new Dropdown.OptionData();
                m_NewData.text = Path.GetFileName(item);
                m_Messages.Add(m_NewData);
            }
            foreach (var item in m_Messages)
            {
                //Add each entry to the Dropdown
                dropdown.options.Add(item);
            }
            dropdown.captionText.text = dropdown.options[0].text;
            selectedFile = dropdown.value;
        }
    }

    public void SelectFile()
    {
        selectedFile = dropdown.value;
    }

    private IEnumerator DisablePopUp()
    {
        yield return new WaitForSecondsRealtime(2);
        popUp.gameObject.SetActive(false);
    }
}
