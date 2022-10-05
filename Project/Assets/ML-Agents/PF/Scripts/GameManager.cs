using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.TrainingStateMachine;
using UnityEngine;

namespace ML_Agents.PF.Scripts
{
    public class GameManager : MonoBehaviour
    {

        [Header("Training Vars")]
        public PhaseType PhaseType;
        public TrainingType TrainingType;

        [Space]
        public RewardData RewardData;

        public TrainingStateMachine.TrainingStateMachine TrainingStateMachine { get; private set; }

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Utils.Utils.GatherAssets();

            TrainingStateMachine = CreateStateMachine();

            if (TrainingStateMachine == null)
            {
                Debug.LogError("State Machine => null");
            }
        }

        private TrainingStateMachine.TrainingStateMachine CreateStateMachine()
        {
            if (TrainingType == TrainingType.Advanced)
            {
                return new AdvancedTraining(PhaseType, TrainingType);
            }

            return new SimpleTraining(PhaseType, TrainingType);
        }
    }
}