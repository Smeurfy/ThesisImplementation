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
public class JsonWriter : MonoBehaviour
{
    public static JsonWriter instance;
    public List<string> _btnClickedOnDeath = new List<string>();
    public List<int> _roomsOfDeath = new List<int>();
    public List<PossibleChallengeData> _skippedChallenges = new List<PossibleChallengeData>();
    public List<PossibleChallengeData> _tryLaterChallenges = new List<PossibleChallengeData>();
    public int _roomClearedCount = 0;

    bool triggerResultEmail = false;
    bool resultEmailSucess;

    private void Awake()
    {
        MakeThisObjectSingleton();
    }

    public void SaveMonsterOrderToFile()
    {
        DateTime dateTime = DateTime.Now;

        string path = Application.dataPath + "/Resources/" + "data_" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + "_" + dateTime.Day + "_" + dateTime.Month + "_" + dateTime.Year + ".json";
        Debug.Log(path);
        FileStream stream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
        {
            writer.WriteLine(SimpleEmailSender.GetEmail());
            writer.WriteLine("Player died on this challenges");
            foreach (var item in _roomsOfDeath)
            {
                writer.WriteLine(item);
            }
            writer.WriteLine("Option selected on death");
            foreach (var item in _btnClickedOnDeath)
            {
                writer.WriteLine(item);
            }
            writer.WriteLine("Skipped challenges");
            foreach (var item in _skippedChallenges)
            {
                writer.WriteLine(item.GetTypeOfEnemies()[0].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[0]] +
                                 item.GetTypeOfEnemies()[1].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[1]]);

            }
            writer.WriteLine("Try later challenges");
            foreach (var item in _tryLaterChallenges)
            {
                writer.WriteLine(item.GetTypeOfEnemies()[0].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[0]] +
                                 item.GetTypeOfEnemies()[1].name + " tier " + DungeonManager.instance.tierOfEnemies[item.GetTypeOfEnemies()[1]]);

            }
            writer.WriteLine("The player cleared " + _roomClearedCount + " room before giving up");
        }
        List<string> paths = new List<string>();
        paths.Add(path);
        SimpleEmailSender.Send(paths, SendCompletedCallback);
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

    private void MakeThisObjectSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
