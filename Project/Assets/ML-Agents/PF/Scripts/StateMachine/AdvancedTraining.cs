using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.UtilsScripts;
using Unity.MLAgents;

namespace ML_Agents.PF.Scripts.StateMachine
{
    public class AdvancedTraining : TrainingStateMachine
    {
        public AdvancedTraining(PhaseType phaseType, TrainingType trainingType) : base(phaseType, trainingType)
        {
            UpdateEndEpisodeConditionsList();
        }

        protected override void EndEpisode()
        {
            PreviousStepReward = 0;
            base.EndEpisode();
        }

        public override void RunOnStepReward()
        {
            base.RunOnStepReward();

            if (HasEpisodeEnded())
            {
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Penalty);
                EndEpisode();
                return;
            }

            var newStepReward = CalculateComplexReward() / RewardData.DivRewardValue;

            if (Utils.NearlyEqual(PreviousStepReward, newStepReward, RewardData.StepRewardFrequency))
            {
                return;
            }

            if (PreviousStepReward > newStepReward)
            {
                var penalty = RewardData.StepPenalty / (100f * RewardData.DivRewardValue);
                GiveInternalReward(RewardUseType.Add_Reward, penalty);
                return;
            }
            PreviousStepReward = newStepReward;
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
                var result = Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength, false);
                DijkstraDataWriter(CHECK_POINT_KEY, result);
                OnTerminalCondition(RewardUseType.Set_Reward);
            }
            else if ((int)PhaseType >= 3)
            {
                var result = Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength, false);
                DijkstraDataWriter(CHECK_POINT_KEY, result);
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
                var result = Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.FullPathLength, false);
                DijkstraDataWriter(FINAL_GOAL_KEY , result);
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
