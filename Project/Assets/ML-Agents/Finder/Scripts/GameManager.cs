using ML_Agents.Finder.Scripts.Enums;
using UnityEngine;


namespace ML_Agents.Finder.Scripts
{
    public class GameManager : MonoBehaviour
    {

        public PhaseType phaseType;
        public TrainingType trainingType;

        public RewardData RewardData;

        public StateMachine _stateMachine { get; private set; }

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _stateMachine = new StateMachine(phaseType, trainingType); //TODO : create state machince
            Utils.Utils.GatherAssets();
        }


        //move this
        public class StateMachine
        {
            public PhaseType PhaseType;
            public TrainingType TrainingType;

            public StateMachine(PhaseType phaseType, TrainingType trainingType)
            {
                PhaseType = phaseType;
                TrainingType = trainingType;
            }
        }
    }
}


