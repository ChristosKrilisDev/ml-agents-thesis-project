using System;
using System.Globalization;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //singleton


    private TextFileHandler fileHandler;
    [SerializeField]
    private int index = 0;
    [SerializeField]
    private bool canWriteData = false;

    //DateTime datetime;
    DateTime localDate = DateTime.Now;

    void Awake()
    {
        if (instance == null)
            instance = this;

        if (!canWriteData)
            return;

            if (!PlayerPrefs.HasKey("Index"))
            PlayerPrefs.SetInt("Index", index);
        else
            index = PlayerPrefs.GetInt("Index");


        PlayerPrefs.SetInt("Index", ++index);


        if (fileHandler == null)
        {
            var culture = new CultureInfo("en-US");
            fileHandler = new TextFileHandler(index + " " + localDate.ToString(culture));
        }
        else
            Debug.Log("Text file handler already exist");
    }



    public void WriteData(float episodeCounter, float agentDistance, float dijkstraDistance, bool hasFindTarget, float avrRewards)
    {
        if (canWriteData)
            fileHandler.WriteString(episodeCounter, agentDistance, dijkstraDistance, hasFindTarget, avrRewards);
    }


}
