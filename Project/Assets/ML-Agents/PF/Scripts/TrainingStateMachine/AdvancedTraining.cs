using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using Unity.MLAgents;
using UnityEngine;

namespace ML_Agents.PF.Scripts.TrainingStateMachine
{
    public class AdvancedTraining : TrainingStateMachine
    {
        private float _previousStepReward;

        public AdvancedTraining(PhaseType phaseType, TrainingType trainingType) : base(phaseType, trainingType)
        {
            HasEndConditions = CreateEndConditionsList();
        }

        protected override void EndEpisode()
        {
            _previousStepReward = 0;
            // _prevPenaltyReward = 0;
            base.EndEpisode();
        }

        public override void RunOnStepReward()
        {
            base.RunOnStepReward();

            var newStepReward = CalculateComplexReward() / RewardData.DivRewardValue;

            if (Utils.Utils.NearlyEqual(_previousStepReward, newStepReward, 0.005f)) return; //increase epsilon

            if (_previousStepReward > newStepReward) //todo fix here
            {
                var penalty = - RewardData.StepPenaltyPerSec / (100f * RewardData.DivRewardValue);
                GiveInternalReward(RewardUseType.Add_Reward, penalty);
                return;
            }
            _previousStepReward = newStepReward;
            GiveInternalReward(RewardUseType.Add_Reward, newStepReward);

            if (HasEpisodeEnded())
            {
                Debug.Log("ended--->");
            }
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

            if (PhaseType == PhaseType.Phase_D) //useless if
            {
                DijkstraDataWriter(ConditionsData.CheckPointPathLength + ConditionsData.FullPathLength, FINAL_GOAL_KEY);
                OnTerminalCondition(RewardUseType.Set_Reward);
            }
        }

        protected override List<bool> CreateEndConditionsList()
        {
            if (TrainingType == TrainingType.Advanced && (int)PhaseType <= 2)
            {
                return new List<bool>
                {
                    ConditionsData.HasFoundCheckpoint,
                    ConditionsData.StepCount == ConditionsData.MaxStep,
                    ConditionsData.HasTouchedWall,
                    Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength)
                };

            }

            return new List<bool>
            {
                ConditionsData.HasFoundGoal,
                ConditionsData.StepCount == ConditionsData.MaxStep,
                ConditionsData.HasTouchedWall,
                Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.FullPathLength + ConditionsData.CheckPointPathLength)
            };
        }

    }
}
