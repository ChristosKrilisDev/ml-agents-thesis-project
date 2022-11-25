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

        [SerializeField] private bool _rlTraining = false;
        [SerializeField, Range(1,15)] private int _areasCount;
        [SerializeField] private GameObject _trainingAreasParent;

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

            AutoActivateTrainingAreas();
        }

        private void AutoActivateTrainingAreas()
        {
            if (!_rlTraining)
                return;

            _trainingAreasParent.SetActive(true);
            var areas = _trainingAreasParent.GetComponentsInChildren<Transform>();

            for (int i = 0; i < _areasCount; i++)
            {
                areas[i].gameObject.SetActive(true);
            }
        }

        public TrainingStateMachine CreateStateMachine()
        {
            return TrainingType == TrainingType.Advanced ?(TrainingStateMachine)
                new AdvancedTraining(PhaseType, TrainingType) :
                new SimpleTraining(PhaseType, TrainingType);
        }
    }
}
