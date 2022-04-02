using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private TextFileHandler fileHandler;

    public static GameManager instance; //singleton

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        if (fileHandler == null)
            fileHandler = new TextFileHandler();
        else
            Debug.Log("TXT file handler already exist");
    }

    public TextFileHandler GetTextFileHandler()
    {
        return fileHandler;
    }

}
