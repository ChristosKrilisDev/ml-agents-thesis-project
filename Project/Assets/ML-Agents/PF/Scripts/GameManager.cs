using System;
using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.StateMachine;
using ML_Agents.PF.Scripts.UtilsScripts;
using UnityEngine;

namespace ML_Agents.PF.Scripts
{
    [Serializable]
    public class RewardDataSets
    {
        [SerializeField] private PhaseType _type;
        public PhaseType Type
        {
            get => _type;
            set => _type = value;
        }
        [SerializeField] private RewardData _rewardData;
        public RewardData RewardData
        {
            get => _rewardData;
            set => _rewardData = value;
        }
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Training Vars")]
        public PhaseType PhaseType;
        public TrainingType TrainingType;
        [Space]
        [SerializeField] private List<RewardDataSets> _rewardDatas;
        [SerializeField] private RewardData _rewardDataFallBack;
        [HideInInspector] public RewardData RewardData;

        [Space]
        [SerializeField] private bool _rlTraining;
        [SerializeField, Range(1, 15)] private int _areasCount;
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

            SetRewardDataFallBack();
        }

        private void SetRewardDataFallBack()
        {
            foreach (var dataSet in _rewardDatas)
            {
                if (dataSet.Type != PhaseType) continue;

                RewardData = dataSet.RewardData;

                return;
            }
            RewardData = _rewardDataFallBack;
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
            return TrainingType == TrainingType.Advanced ? (TrainingStateMachine)
                new AdvancedTraining(PhaseType, TrainingType) :
                new SimpleTraining(PhaseType, TrainingType);
        }
    }
}
