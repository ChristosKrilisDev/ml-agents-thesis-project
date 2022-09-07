using System;
using System.Globalization;
using ML_Agents.Finder.Scripts;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum TrainingType
    {
        Simple = 1,
        Advanced = 2,
    }
    public enum PhaseType
    {
        Phase_A = 1,
        Phase_B = 2,
        Phase_C = 3,
        Phase_D = 4,
    }

    public enum ExtraRewardType
    {
        Use_Null=0,
        Use_Step_Reward = 1,
        Use_Dijkstra_Reward = 2,
        Use_Both = 3
    }

    public PhaseType phaseType;
    public TrainingType trainingType;
    public ExtraRewardType extraRewardType;


    [SerializeField] private bool _canWriteData = false;
    private TextFileHandler _fileHandler;
    private DateTime _localDate = DateTime.Now;
    private int _index;

    public StateMachine _stateMachine { get; private set; }

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _stateMachine = new StateMachine(phaseType, trainingType, extraRewardType);
        EpisodeHandler.Init();


        // if (!_canWriteData) return;
        //
        // if (!PlayerPrefs.HasKey("Index")) PlayerPrefs.SetInt("Index", _index);
        // else _index = PlayerPrefs.GetInt("Index");
        //
        // PlayerPrefs.SetInt("Index", ++_index);
        //
        // if (_fileHandler == null)
        // {
        //     var culture = new CultureInfo("en-US");
        //     _fileHandler = new TextFileHandler(_index + " " + _localDate.ToString(culture));
        // }
        // else Debug.Log("Text file handler already exist");
    }

    public void WriteData(float episodeCounter, float agentDistance, float dijkstraDistance, bool hasFindTarget, float avrRewards)
    {
        if (_canWriteData)
            TextFileHandler.WriteString(episodeCounter, agentDistance, dijkstraDistance, hasFindTarget, avrRewards);
    }




    public class StateMachine
    {
        public PhaseType PhaseType;
        public TrainingType TrainingType;
        public ExtraRewardType ExtraRewardType;

        public StateMachine(PhaseType phaseType, TrainingType trainingType, ExtraRewardType extraRewardType)
        {
            PhaseType = phaseType;
            TrainingType = trainingType;
            ExtraRewardType = extraRewardType;

        }
    }
}


