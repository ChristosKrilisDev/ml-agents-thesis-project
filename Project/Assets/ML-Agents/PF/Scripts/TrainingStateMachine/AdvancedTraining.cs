using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
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

        public override void RunOnStepReward()
        {
            base.RunOnStepReward();


            // _stepFactor = (_stepFactor - MaxStep) / MaxStep;
            // RunGiveRewardInternal(RewardUseType.Add_Reward, -_stepFactor);

            var newStepReward = RunCalculateComplexReward();
            if (Utils.Utils.NearlyEqual(_previousStepReward, newStepReward, 0.001f)) return;

            if (_previousStepReward > newStepReward)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, -GameManager.Instance.RewardData.StepReward / 10);
                return;
            }

            _previousStepReward = newStepReward;
            RunGiveRewardInternal(RewardUseType.Add_Reward, newStepReward);

            Debug.Log("Step Reward : " + newStepReward);
        }

        public override void RunOnCheckPointReward()
        {
            base.RunOnCheckPointReward();

            RunGiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward);

            if ((int)PhaseType == 1)
            {
                RunOnTerminalCondition(RewardUseType.Set_Reward);
            }
            else if ((int)PhaseType == 2)
            {
                RunDijkstraDataWriter(ConditionsData.CheckPointLength, CHECK_POINT_KEY);
                RunOnTerminalCondition(RewardUseType.Set_Reward);
            }
            else if ((int)PhaseType >= 3)
            {
                RunDijkstraDataWriter(ConditionsData.CheckPointLength, CHECK_POINT_KEY);
                RunGiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward);
                // SwitchTargetNodeCallBack?.Invoke();
            }
        }

        public override void RunOnFinalGoalReward()
        {
            base.RunOnFinalGoalReward();

            Academy.Instance.StatsRecorder.Add("Distance/Distance Traveled", ConditionsData.TraveledDistance, StatAggregationMethod.Histogram);
            Academy.Instance.StatsRecorder.Add("Distance/Shortest Path", ConditionsData.PathTotalLength, StatAggregationMethod.Histogram);

            RunGiveRewardInternal(RewardUseType.Add_Reward, RewardData.Reward);

            if ((int)PhaseType == 3)
            {
                RunOnTerminalCondition(RewardUseType.Set_Reward);
            }
            else if ((int)PhaseType == 4)
            {
                RunDijkstraDataWriter(ConditionsData.CheckPointLength + ConditionsData.PathTotalLength, FINAL_GOAL_KEY);
                RunOnTerminalCondition(RewardUseType.Set_Reward);
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
                    Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointLength)
                };

            }

            return new List<bool>
            {
                ConditionsData.HasFoundGoal,
                ConditionsData.StepCount == ConditionsData.MaxStep,
                ConditionsData.HasTouchedWall,
                Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.PathTotalLength + ConditionsData.CheckPointLength)
            };
        }

    }
}
