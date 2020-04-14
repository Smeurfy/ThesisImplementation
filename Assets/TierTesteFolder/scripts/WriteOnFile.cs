using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class WriteOnFile : MonoBehaviour
{
    public event Action OnFileSaved = delegate { };
    public Canvas canvas;
    public Text popUp;
    public void SaveDataToFile()
    {
        DateTime dateTime = DateTime.Now;
        var dic = canvas.GetComponent<PopulateWithMonsters>().monstersInfo;
        string path = Application.dataPath + "/SAVEDFILES/" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Millisecond + "_" + dateTime.Day + dateTime.Month + dateTime.Year + ".json";
        FileStream stream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
        {
            foreach (var item in dic)
            {
                foreach (var item1 in item.Value)
                {
                    writer.WriteLine(JsonUtility.ToJson(item1.Value));
                }
            }
        }
        popUp.gameObject.SetActive(true);
        StartCoroutine(DisablePopUp());
        OnFileSaved();
    }

    private IEnumerator DisablePopUp()
    {
        yield return new WaitForSecondsRealtime(2);
        popUp.gameObject.SetActive(false);
    }
}




