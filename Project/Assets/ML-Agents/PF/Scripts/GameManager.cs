using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.StateMachine;
using ML_Agents.PF.Scripts.UtilsScripts;
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

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Utils.GatherAssets();

        }

        public TrainingStateMachine CreateStateMachine()
        {
            return TrainingType == TrainingType.Advanced ?(TrainingStateMachine)
                new AdvancedTraining(PhaseType, TrainingType) :
                new SimpleTraining(PhaseType, TrainingType);
        }
    }
}
