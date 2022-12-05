using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.UtilsScripts;

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

            //todo : fix that shit
            var reward = RewardData.StepPenaltyPerSec / (ConditionsData.MaxStep / (ConditionsData.MaxStep * 1f));

            if (PhaseType is PhaseType.Phase_B || PhaseType is PhaseType.Phase_C)
            {
                GiveInternalReward(RewardUseType.Add_Reward, reward / 1000f); //0.0001f /-0.15f
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                GiveInternalReward(RewardUseType.Add_Reward, reward / 100f); //0.001f/-1.5f
            }
        }

        public override void RunOnCheckPointReward()
        {
            base.RunOnCheckPointReward();

            if (PhaseType == PhaseType.Phase_A)
            {
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
                EndEpisode();
            }
            else
            {
                //create dijkstra analytics
                if (PhaseType == PhaseType.Phase_B)
                {
                    GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward / 2);

                    //todo : fix that shit
                    //give reward per node
                    if (Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength, false))
                    {
                        GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
                    }
                    DijkstraDataWriter(ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);

                    EndEpisode();
                }
                else if (PhaseType == PhaseType.Phase_C || PhaseType == PhaseType.Phase_D)
                {
                    GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward / 3);
                }

                if (Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength, false))
                {
                    GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
                }
                DijkstraDataWriter(ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
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
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward / 2);

                DijkstraDataWriter(ConditionsData.FullPathLength, FINAL_GOAL_KEY);

                if (Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.FullPathLength))
                {
                    GiveInternalReward(RewardUseType.Set_Reward, 1f);
                }
            }
            EndEpisode();
        }

        protected override void UpdateEndEpisodeConditionsList()
        {
            if(TrainingType != TrainingType.Simple) return;

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
