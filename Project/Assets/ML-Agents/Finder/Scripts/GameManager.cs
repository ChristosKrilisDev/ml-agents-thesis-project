using UnityEngine;


namespace ML_Agents.Finder.Scripts
{
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

        public PhaseType phaseType;
        public TrainingType trainingType;

        public RewardData RewardData;

        public StateMachine _stateMachine { get; private set; }

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _stateMachine = new StateMachine(phaseType, trainingType);
            EpisodeHandler.Init();
        }

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


