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

        [Space]
        [SerializeField] private bool _rlTraining = false;
        [SerializeField, Range(1,15)] private int _areasCount;
        [SerializeField] private GameObject _trainingAreasParent;
        [SerializeField] private GameObject _testArea;

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

            _testArea.SetActive(false);
            _trainingAreasParent.SetActive(true);

            for (int i = 0; i < _areasCount; i++)
            {
                var area = _trainingAreasParent.transform.GetChild(i);
                area.gameObject.SetActive(true);
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
