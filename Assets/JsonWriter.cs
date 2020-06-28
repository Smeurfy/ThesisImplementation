using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JsonWriter : MonoBehaviour
{
    public static JsonWriter instance;

    private void Awake()
    {
        MakeThisObjectSingleton();
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
