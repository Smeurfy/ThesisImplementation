﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowMonsterTier : MonoBehaviour
{

    private List<List<Image>> gridLayout = new List<List<Image>>();
    public List<Sprite> monstersImages = new List<Sprite>();

    public Image image, background;
    private GameObject gO;

    public int x, y;
    public Vector3 startingPosition;

    // Use this for initialization
    void Start()
    {
        x = (int)background.GetComponent<RectTransform>().rect.width;
        y = (int)background.GetComponent<RectTransform>().rect.height;
        startingPosition = background.GetComponent<RectTransform>().position;
        startingPosition.x = startingPosition.x - x / 2 + 33;
        startingPosition.y = startingPosition.y + y / 2 - 33;
        Populate();
        gameObject.SetActive(false);
    }

    private void Populate()
    {
        int auxX = x / 7;
        int auxY = y / (EnemyLibrary.instance.GetAllPossibleEnemies().Count + 1);

        Vector3 incrY = Vector3.zero;
        for (int i = 0; i <= EnemyLibrary.instance.GetAllPossibleEnemies().Count; i++)
        {
            gridLayout.Add(new List<Image>());
            Vector3 incrX = Vector3.zero;
            for (int j = 0; j < 7; j++)
            {

                var obj = Instantiate(image, startingPosition + incrX + incrY, Quaternion.identity, gameObject.transform);
                //Top row
                if (i == 0)
                {
                    if (j == 0)
                    {
                        obj.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                        obj.GetComponentInChildren<Text>().text = "Monsters";
                        obj.GetComponentInChildren<Text>().fontSize = (auxY - 10) / (obj.GetComponentInChildren<Text>().text.Length) * 10 / ((auxY - 10) / 8);
                    }
                    else
                    {
                        obj.GetComponentInChildren<Text>().text = "Tier " + (j - 1);
                        obj.GetComponentInChildren<Text>().fontSize = (auxY - 10) / (obj.GetComponentInChildren<Text>().text.Length) * 10 / ((auxY - 10) / 8);
                    }
                }
                // first Column
                if (i != 0 && j == 0)
                {
                    obj.GetComponent<Image>().sprite = monstersImages[i - 1];
                    obj.GetComponent<Image>().name = obj.GetComponent<Image>().sprite.name;
                    obj.GetComponentInChildren<Text>().enabled = false;
                }
                if (i != 0 && j != 0)
                {
                    obj.GetComponent<Image>().color = new Color(1f, 0f, 0f);
                    obj.GetComponentInChildren<Text>().enabled = false;
                }
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(auxY - 10, auxY - 10);
                obj.gameObject.SetActive(true);
                gridLayout[i].Add(obj);
                incrX += new Vector3(auxX, 0, 0);
            }
            incrY -= new Vector3(0, auxY, 0);
        }
    }

    public void ChangeColor(string name, int tier)
    {
        gameObject.SetActive(true);
        var aux = gridLayout[0];
        bool a = true;
        for (int i = 1; i <= EnemyLibrary.instance.GetAllPossibleEnemies().Count; i++)
        {
            if (a)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (string.Compare(gridLayout[i][j].name, name) == 0)
                    {
                        aux = gridLayout[i];
                        a = false;
                    }
                }
            }

        }
        for (int i = 0; i < 7  ; i++)
        {
            if (i != 0 && (i - 1) < tier)
            {
                Debug.Log("bora bora");
                aux[i].GetComponent<Image>().color = new Color(0f, 1f, 0f);
            }

        }
        gameObject.SetActive(false);
    }
}
