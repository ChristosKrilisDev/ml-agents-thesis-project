using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.UtilsScripts;
using Unity.MLAgents;
using UnityEngine;

namespace ML_Agents.PF.Scripts.StateMachine
{
    public class AdvancedTraining : TrainingStateMachine
    {
        private float _previousStepReward;

        public AdvancedTraining(PhaseType phaseType, TrainingType trainingType) : base(phaseType, trainingType)
        {
            UpdateEndEpisodeConditionsList();
        }

        protected override void EndEpisode()
        {
            _previousStepReward = 0;
            base.EndEpisode();
        }

        public override void RunOnStepReward()
        {
            base.RunOnStepReward();

            if (HasEpisodeEnded())
            {
                Debug.Log("Max Length or Max Steps reached");
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Penalty);
                EndEpisode();
                return;
            }

            var newStepReward = CalculateComplexReward() / RewardData.DivRewardValue;
            
            if (Utils.NearlyEqual(_previousStepReward, newStepReward, RewardData.StepRewardFrequency))
            {
                return;
            }

            if (_previousStepReward > newStepReward)
            {
                var penalty = - RewardData.StepPenaltyPerSec / (100f * RewardData.DivRewardValue);
                GiveInternalReward(RewardUseType.Add_Reward, penalty);
                return;
            }
            _previousStepReward = newStepReward;
            GiveInternalReward(RewardUseType.Add_Reward, newStepReward);
        }

        public override void RunOnCheckPointReward()
        {
            base.RunOnCheckPointReward();

            GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward);

            if (PhaseType == PhaseType.Phase_A)
            {
                OnTerminalCondition(RewardUseType.Set_Reward);
            }
            else if (PhaseType == PhaseType.Phase_B)
            {
                DijkstraDataWriter(ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
                OnTerminalCondition(RewardUseType.Set_Reward);
            }
            else if ((int)PhaseType >= 3)
            {
                DijkstraDataWriter(ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward);
            }
        }

        public override void RunOnFinalGoalReward()
        {
            base.RunOnFinalGoalReward();

            Academy.Instance.StatsRecorder.Add("Distance/Distance Traveled", ConditionsData.TraveledDistance, StatAggregationMethod.Histogram);
            Academy.Instance.StatsRecorder.Add("Distance/Shortest Path", ConditionsData.FullPathLength, StatAggregationMethod.Histogram);

            GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward);

            if (PhaseType == PhaseType.Phase_C)
            {
                OnTerminalCondition(RewardUseType.Set_Reward);
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                DijkstraDataWriter(ConditionsData.CheckPointPathLength + ConditionsData.FullPathLength, FINAL_GOAL_KEY);
                OnTerminalCondition(RewardUseType.Set_Reward);
            }
        }

        protected override void UpdateEndEpisodeConditionsList()
        {
            if(TrainingType != TrainingType.Advanced) return; //fix

            if ((int)PhaseType <= 2)
            {
                EndEpisodeConditions = new List<bool>()
                {
                    ConditionsData.StepCount == ConditionsData.MaxStep,
                    ConditionsData.HasFoundCheckpoint,
                    !Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength)
                };

            }
            else
            {
                EndEpisodeConditions = new List<bool>()
                {
                    ConditionsData.StepCount == ConditionsData.MaxStep,
                    ConditionsData.HasFoundGoal,
                    !Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.FullPathLength)
                };
            }
        }

    }
}
