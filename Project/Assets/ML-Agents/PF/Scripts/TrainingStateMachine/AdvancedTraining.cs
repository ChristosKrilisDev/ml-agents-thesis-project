using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using Unity.MLAgents;
using UnityEngine;

namespace ML_Agents.PF.Scripts.TrainingStateMachine
{
    public class AdvancedTraining : TrainingStateMachine
    {
        private float _previousStepReward;
        // private float _prevPenaltyReward;


        public AdvancedTraining(PhaseType phaseType, TrainingType trainingType) : base(phaseType, trainingType)
        {
            HasEndConditions = CreateEndConditionsList();
        }


        protected override void EpisodeEndCallBack()
        {
            _previousStepReward = 0;
            // _prevPenaltyReward = 0;
            base.EpisodeEndCallBack();
        }


        public override void RunOnStepReward()
        {
            base.RunOnStepReward();

            ConditionsData.StepFactor = (ConditionsData.StepCount - ConditionsData.MaxStep) / (float)ConditionsData.MaxStep;
            // Debug.Log(ConditionsData.StepFactor);
            // RunGiveRewardInternal(RewardUseType.Add_Reward, -_stepFactor);

            var newStepReward = CalculateComplexRewardCallBack() / RewardData.DivRewardValue;

            if (Utils.Utils.NearlyEqual(_previousStepReward, newStepReward, 0.01f)) return; //increase epsilon


            if (_previousStepReward > newStepReward) //fix here
            {
                GiveRewardInternalCallBack(RewardUseType.Add_Reward, -RewardData.StepPenaltyPerSec / (100f * RewardData.DivRewardValue));

                // var newPenaltyReward = -1f * Mathf.Abs(RewardData.StepPenaltyPerSec / (100f * RewardData.DivRewardValue) + newStepReward);
                // if (Utils.Utils.NearlyEqual(_prevPenaltyReward, newPenaltyReward, 0.001f)) return;
                // // Debug.Log("-----d "+newPenaltyReward);
                //
                // if (_prevPenaltyReward > newPenaltyReward)
                // {
                //     _prevPenaltyReward = newPenaltyReward;
                //     GiveRewardInternalCallBack(RewardUseType.Add_Reward, _prevPenaltyReward);
                // }

                return;
            }

            _previousStepReward = newStepReward;
            GiveRewardInternalCallBack(RewardUseType.Add_Reward, newStepReward);
        }

        public override void RunOnCheckPointReward()
        {
            base.RunOnCheckPointReward();

            GiveRewardInternalCallBack(RewardUseType.Add_Reward, RewardData.Reward);

            if (PhaseType == PhaseType.Phase_A)
            {
                OnTerminalConditionCallBack(RewardUseType.Set_Reward);
            }
            else if (PhaseType == PhaseType.Phase_B)
            {
                DijkstraDataWriter(ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
                OnTerminalConditionCallBack(RewardUseType.Set_Reward);
            }
            else if ((int)PhaseType >= 3)
            {
                DijkstraDataWriter(ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
                GiveRewardInternalCallBack(RewardUseType.Add_Reward, RewardData.Reward);
            }
        }

        public override void RunOnFinalGoalReward()
        {
            base.RunOnFinalGoalReward();

            Academy.Instance.StatsRecorder.Add("Distance/Distance Traveled", ConditionsData.TraveledDistance, StatAggregationMethod.Histogram);
            Academy.Instance.StatsRecorder.Add("Distance/Shortest Path", ConditionsData.FullPathLength, StatAggregationMethod.Histogram);

            GiveRewardInternalCallBack(RewardUseType.Add_Reward, RewardData.Reward);

            if (PhaseType == PhaseType.Phase_C)
            {
                OnTerminalConditionCallBack(RewardUseType.Set_Reward);
            }

            if (PhaseType == PhaseType.Phase_D)
            {
                DijkstraDataWriter(ConditionsData.CheckPointPathLength + ConditionsData.FullPathLength, FINAL_GOAL_KEY);
                OnTerminalConditionCallBack(RewardUseType.Set_Reward);
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
