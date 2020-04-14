﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class WriteOnFile : MonoBehaviour
{
    public Canvas canvas;
    public Text popUp;
    public void SaveDataToFile()
    {
        var dic = canvas.GetComponent<PopulateWithMonsters>().monstersInfo;
        string path = Application.dataPath + "/SAVEDFILES/denis.json";
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
    }

    private IEnumerator DisablePopUp()
    {
        yield return new WaitForSecondsRealtime(2);
        popUp.gameObject.SetActive(false);
    }
}



