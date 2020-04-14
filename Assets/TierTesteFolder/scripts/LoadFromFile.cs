using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class LoadFromFile : MonoBehaviour
{
    public Canvas canvas;
    public Dropdown dropdown;
    //The list of messages for the Dropdown
    List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
    Dropdown.OptionData m_NewData;
    int selectedFile;

    void Start()
    {

        InitDropDown();
    }

    public void LoadData()
    {
        var dic = canvas.GetComponent<PopulateWithMonsters>().monstersInfo;
        string path = Application.dataPath + "/SAVEDFILES/" + dropdown.options[selectedFile].text;
        string[] fileContent = File.ReadAllLines(path);
        foreach (var item in dic)
        {
            foreach (var str in fileContent)
            {
                MonstersInfo obj = JsonUtility.FromJson<MonstersInfo>(str);
                if (obj.monsterName == item.Key)
                {
                    item.Value.Clear();
                    item.Value.Add(obj.tier, obj);
                }
            }
        }
        canvas.GetComponent<PopulateWithMonsters>().monstersInfo = dic;
        canvas.GetComponent<PopulateWithMonsters>().DestroyMonster();
        canvas.GetComponent<PopulateWithMonsters>().UnselectBtn();
        canvas.GetComponent<PopulateWithMonsters>().Populate();
    }

    private void InitDropDown()
    {
        dropdown.ClearOptions();
        string[] a = Directory.GetFiles(Application.dataPath + "/SAVEDFILES/", "*.json");
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

    public void SelectFile()
    {
        selectedFile = dropdown.value;
    }
}
