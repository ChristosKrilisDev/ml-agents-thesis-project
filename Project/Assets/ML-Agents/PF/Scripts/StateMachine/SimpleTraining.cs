using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;

namespace ML_Agents.PF.Scripts.StateMachine
{
    public class SimpleTraining : TrainingStateMachine
    {
        public SimpleTraining(PhaseType phaseType, TrainingType trainingType) : base(phaseType, trainingType)
        {
            UpdateEndEpisodeConditionsList();
        }

        public override void RunOnStepReward()
        {
            base.RunOnStepReward();

            if (PhaseType == PhaseType.Phase_A) return;

            var reward = RewardData.StepPenalty / ConditionsData.MaxStep;

            if (PhaseType is PhaseType.Phase_B) GiveInternalReward(RewardUseType.Add_Reward, reward);
            else if (PhaseType is PhaseType.Phase_C) GiveInternalReward(RewardUseType.Add_Reward, reward);
            else if (PhaseType == PhaseType.Phase_D) GiveInternalReward(RewardUseType.Add_Reward, reward);
        }

        public override void RunOnCheckPointReward()
        {
            base.RunOnCheckPointReward();

            if (PhaseType == PhaseType.Phase_A)
            {
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
                EndEpisode();
            }
            else if (PhaseType == PhaseType.Phase_B)
            {
                //todo : give reward per node?
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward/2f);
                DijkstraReward(RewardUseType.Set_Reward, ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
                EndEpisode();
            }
            else if (PhaseType == PhaseType.Phase_C || PhaseType == PhaseType.Phase_D)
            {
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward);
                DijkstraReward(RewardUseType.Add_Reward, ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
            }
        }

        public override void RunOnFinalGoalReward()
        {
            base.RunOnFinalGoalReward();

            if (PhaseType == PhaseType.Phase_C)
            {
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward);
                DijkstraReward(RewardUseType.Set_Reward, ConditionsData.FullPathLength, FINAL_GOAL_KEY);
            }
            EndEpisode();
        }

        protected override void UpdateEndEpisodeConditionsList()
        {
            if (TrainingType != TrainingType.Simple) return;

            EndEpisodeConditions = new List<bool>()
            {
                ConditionsData.HasFoundCheckpoint,
                ConditionsData.StepCount == ConditionsData.MaxStep,
            };

            EndEpisodeConditions = new List<bool>()
            {
                ConditionsData.HasFoundGoal,
                ConditionsData.StepCount == ConditionsData.MaxStep,
            };
        }

    }
}
