using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using ML_Agents.Finder.Scripts;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //singleton
    
    [SerializeField] private bool _canWriteData = false;
    private TextFileHandler _fileHandler;
    private DateTime _localDate = DateTime.Now;
    private int _index = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        EpisodeHandler.Init();
        


        if (!_canWriteData) return;

        if (!PlayerPrefs.HasKey("Index")) PlayerPrefs.SetInt("Index", _index);
        else _index = PlayerPrefs.GetInt("Index");

        PlayerPrefs.SetInt("Index", ++_index);

        if (_fileHandler == null)
        {
            var culture = new CultureInfo("en-US");
            _fileHandler = new TextFileHandler(_index + " " + _localDate.ToString(culture));
        }
        else Debug.Log("Text file handler already exist");
    }



    public void WriteData(float episodeCounter, float agentDistance, float dijkstraDistance, bool hasFindTarget, float avrRewards)
    {
        if (_canWriteData)
            _fileHandler.WriteString(episodeCounter, agentDistance, dijkstraDistance, hasFindTarget, avrRewards);
    }


}
