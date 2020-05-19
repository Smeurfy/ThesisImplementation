using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text;
using System.ComponentModel;
using UnityEngine.SceneManagement;

public class WriteOnFile : MonoBehaviour
{
    bool triggerResultEmail = false;
    bool resultEmailSucess;
    public event Action OnFileSaved = delegate { };
    public Canvas canvas;
    public Text popUp;
    public void SaveDataToFile()
    {
        DateTime dateTime = DateTime.Now;
        var dic = canvas.GetComponent<PopulateWithMonsters>().monstersInfo;
        string path = Application.dataPath + "/Resources/" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + "_" + dateTime.Day + "_" + dateTime.Month + "_" + dateTime.Year + ".json";
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

    public void SaveMonsterOrderToFile()
    {
        DateTime dateTime = DateTime.Now;
        var dic = canvas.GetComponent<UiController>().order;
        string path = Application.dataPath + "/Resources/" + "order_" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + "_" + dateTime.Day + "_" + dateTime.Month + "_" + dateTime.Year + ".json";
        Debug.Log(path);
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
        string secondPath = SaveLogs();
        List<string> paths = new List<string>();
        paths.Add(path);
        paths.Add(secondPath);
        SimpleEmailSender.Send(paths, SendCompletedCallback);
        popUp.gameObject.SetActive(true);
        StartCoroutine(DisablePopUp());
    }

    string SaveLogs()
    {
        DateTime dateTime = DateTime.Now;
        var logs = canvas.GetComponent<UiController>().logs;
        string path = Application.dataPath + "/Resources/" + "log_" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + "_" + dateTime.Day + "_" + dateTime.Month + "_" + dateTime.Year + ".json";
        FileStream stream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
        {
            foreach (var item in logs)
            {
                foreach (var item1 in item.Value)
                {
                    writer.WriteLine(item1);
                }
            }
        }
        return path;
    }

    private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled || e.Error != null)
        {
            print("Email not sent: " + e.Error.ToString());

            resultEmailSucess = false;
            triggerResultEmail = true;
        }
        else
        {
            print("Email successfully sent.");

            resultEmailSucess = true;
            triggerResultEmail = true;
        }
    }

    private IEnumerator DisablePopUp()
    {
        yield return new WaitForSecondsRealtime(2);
        popUp.gameObject.SetActive(false);
        ThankYouScene();
    }

    void ThankYouScene()
    {
        Destroy(GameObject.Find("playerTest"));
        SceneManager.LoadScene(3);
    }

}


