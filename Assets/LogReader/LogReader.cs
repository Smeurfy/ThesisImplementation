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

public class LogReader : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, MonstersInfo>> _monstersInfo;
    private Dictionary<string, OrderData> _finalData;
    private string[] _logs;
    private string[] _orders;
    private List<string> _enemies = new List<string>();

    // Use this for initialization
    void Start()
    {

        GetEnemiesNames();
        _monstersInfo = new Dictionary<string, Dictionary<string, MonstersInfo>>();
        _finalData = new Dictionary<string, OrderData>();
        InitializeDic();
        LoadFile();
        ReadLogs();
        ReadOrders();
        SaveDataToFile();
    }

    private void InitializeDic()
    {
        foreach (var enemy in _enemies)
        {
            _finalData.Add(enemy, new OrderData());
        }
    }

    private void GetEnemiesNames()
    {
        var aux = EnemyLibrary.instance.GetAllPossibleEnemiesPrefabs();
        foreach (var item in aux)
        {
            _enemies.Add(item.name);
        }
    }

    void LoadFile()
    {
        string path = Application.dataPath + "/LogReader";
        _logs = Directory.GetFiles(path, "log_*.json");
        _orders = Directory.GetFiles(path, "order_*.json");
    }

    private void ReadLogs()
    {
        string monster, line;
        foreach (var log in _logs)
        {
            StreamReader file = new StreamReader(log);
            while ((line = file.ReadLine()) != null)
            {
                if (_enemies.Contains(line))
                {
                    monster = line;
                    List<string> monstersSelected = new List<string>();
                    List<string> orderAssigned = new List<string>();
                    Debug.Log(line);
                    while (!((line = file.ReadLine()).Split(' ')[0] == "Total"))
                    {
                        //Select block
                        while (line == "Select 0" || line == "Select 1" || line == "Select 2")
                        {
                            monstersSelected.Add(line);
                            line = file.ReadLine();
                        }
                        if (monstersSelected.Contains("Select 0") && monstersSelected.Contains("Select 1") && monstersSelected.Contains("Select 2"))
                        {
                            _finalData[monster]._allSelected++;
                        }
                        //Order block
                        int count = 0;
                        while (count < 3 && line.Split(' ')[0] == "Order")
                        {
                            orderAssigned.Add(line);
                            if(count != 2 )
                                line = file.ReadLine();
                            count++;
                        }
                        for (int i = 0; i < orderAssigned.Count; i = i + 3)
                        {   
                            //First pick
                            var order = orderAssigned[i].Split(' ')[2];
                            string keyName = "First pick: " + order;
                            if(!_finalData[monster]._orderPick.ContainsKey(keyName))
                                _finalData[monster]._orderPick.Add(keyName, 0);
                            _finalData[monster]._orderPick[keyName]++;

                            //Second pick
                            order = orderAssigned[i+1].Split(' ')[2];
                            keyName = "Second pick: " + order;
                            if(!_finalData[monster]._orderPick.ContainsKey(keyName))
                                _finalData[monster]._orderPick.Add(keyName, 0);
                            _finalData[monster]._orderPick[keyName]++;

                            //Third pick
                            order = orderAssigned[i+2].Split(' ')[2];
                            keyName = "Third pick: " + order;
                            if(!_finalData[monster]._orderPick.ContainsKey(keyName))
                                _finalData[monster]._orderPick.Add(keyName, 0);
                            _finalData[monster]._orderPick[keyName]++;
                        }
                    }
                    //Time
                    _finalData[monster]._totalTime += float.Parse(line.Split(' ')[2]);
                }
            }
        }

    }

    private void ReadOrders()
    {
        foreach (var order in _orders)
        {
            string[] fileContent = File.ReadAllLines(order);
            foreach (var str in fileContent)
            {
                try
                {
                    PlaceholderTier obj = JsonUtility.FromJson<PlaceholderTier>(str);
                    if (!_finalData.ContainsKey(obj.monsterName))
                    {
                        _finalData.Add(obj.monsterName, new OrderData());
                    }
                    _finalData[obj.monsterName]._monstDifferent.Add(obj.tierName, 0);
                }
                catch (ArgumentException e)
                {
                    Debug.Log("Erro: " + str + " " + e);
                    continue;
                }
            }

            //after the file is read
            //count number of correct order for monsters
            for (int i = 1; i < fileContent.Length; i = i + 3)
            {
                try
                {
                    PlaceholderTier obj = JsonUtility.FromJson<PlaceholderTier>(fileContent[i]);
                    PlaceholderTier obj1 = JsonUtility.FromJson<PlaceholderTier>(fileContent[i + 1]);
                    PlaceholderTier obj2 = JsonUtility.FromJson<PlaceholderTier>(fileContent[i + 2]);
                    if ((obj.tierName == "default" && obj.orderNumber == 1 || obj.tierName == "Tier1" && obj.orderNumber == 2 || obj.tierName == "Tier2" && obj.orderNumber == 3) &&
                        (obj1.tierName == "default" && obj1.orderNumber == 1 || obj1.tierName == "Tier1" && obj1.orderNumber == 2 || obj1.tierName == "Tier2" && obj1.orderNumber == 3) &&
                        (obj2.tierName == "default" && obj2.orderNumber == 1 || obj2.tierName == "Tier1" && obj2.orderNumber == 2 || obj2.tierName == "Tier2" && obj2.orderNumber == 3))
                    {
                        _finalData[obj.monsterName]._correctOrder++;
                    }

                }
                catch (ArgumentException e)
                {
                    Debug.Log("Erro: " + e);
                    continue;
                }
            }

            //count the most different monster 
            for (int i = 1; i < fileContent.Length; i++)
            {
                try
                {
                    PlaceholderTier obj = JsonUtility.FromJson<PlaceholderTier>(fileContent[i]);
                    if (obj.mostDifferent == true)
                    {
                        _finalData[obj.monsterName]._monstDifferent[obj.tierName]++;
                    }

                }
                catch (ArgumentException e)
                {
                    Debug.Log("Erro: " + e);
                    continue;
                }
            }



        }

    }

     public void SaveDataToFile()
    {
        DateTime dateTime = DateTime.Now;
        string path = Application.dataPath + "/Resources/" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + "_" + dateTime.Day + "_" + dateTime.Month + "_" + dateTime.Year + ".json";
        FileStream stream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
        {
            foreach (var item in _finalData)
            {
                writer.WriteLine(item.Key);
                writer.WriteLine("Most different version for each monster");
                foreach (var enemy in item.Value._monstDifferent)
                {
                    writer.WriteLine(enemy.Key + " - " + enemy.Value.ToString());
                }
                writer.WriteLine("How many people assigned the right order:");
                writer.WriteLine(item.Value._correctOrder);
                writer.WriteLine("Median time to order the monster");
                writer.WriteLine(item.Value._totalTime/6f + "seconds");
                writer.WriteLine("How many people viewed all version before assigning an order");
                writer.WriteLine(item.Value._allSelected);
                writer.WriteLine("What was the first version to order by the users");
                foreach (var enemy in item.Value._orderPick)
                {
                    writer.WriteLine(enemy.Key + " - " + enemy.Value.ToString());
                }
                writer.WriteLine(" ");
            }
            Debug.Log("Done.");
        }
    }
}
