using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using UnityEngine;

namespace ML_Agents.PF.Scripts.TrainingStateMachine
{
    public class SimpleTraining : TrainingStateMachine
    {

        public SimpleTraining(PhaseType phaseType, TrainingType trainingType) : base(phaseType, trainingType)
        {
            HasEndConditions = CreateEndConditionsList();
        }

        public override void RunOnStepReward()
        {
            base.RunOnStepReward(); //remove this?

            if (PhaseType == PhaseType.Phase_A) return;

            var reward = -RewardData.StepPenaltyPerSec / (ConditionsData.MaxStep / (ConditionsData.MaxStep * 1f));
            Debug.Log("reward : " + reward);

            if ((PhaseType is PhaseType.Phase_B) || PhaseType is PhaseType.Phase_C)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, reward * 10f);  //0.1f
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, reward * 100f); //0.01f
            }

        } //done

        public override void RunOnCheckPointReward() //done
        {
            base.RunOnCheckPointReward();

            if (PhaseType == PhaseType.Phase_A)
            {
                RunGiveRewardInternal(RewardUseType.Set_Reward, RewardData.Reward);
                RunEndEpisodeCallBack();
            }
            else
            {
                if (PhaseType == PhaseType.Phase_B)
                {
                    RunGiveRewardInternal(RewardUseType.Add_Reward, RewardData.Reward / 2);
                    RunEndEpisodeCallBack();
                }
                else if (PhaseType == PhaseType.Phase_C || PhaseType == PhaseType.Phase_D)
                {
                    RunGiveRewardInternal(RewardUseType.Add_Reward, RewardData.Reward / 3);

                }

                RunDijkstraDataWriter(ConditionsData.CheckPointLength, CHECK_POINT_KEY);
                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointLength))
                {
                    RunGiveRewardInternal(RewardUseType.Set_Reward, RewardData.Reward);
                }
            }
        }

        public override void RunOnFinalGoalReward() //done
        {
            base.RunOnFinalGoalReward();

            if (PhaseType == PhaseType.Phase_C)
            {
                RunGiveRewardInternal(RewardUseType.Set_Reward, RewardData.Reward);
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, RewardData.Reward/2);

                RunDijkstraDataWriter(ConditionsData.PathTotalLength, FINAL_GOAL_KEY);
                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointLength + ConditionsData.PathTotalLength))
                {
                    RunGiveRewardInternal(RewardUseType.Set_Reward, 1f);
                }
            }

            RunEndEpisodeCallBack();
        }

        protected override List<bool> CreateEndConditionsList() // done
        {
            return new List<bool>
            {
                ConditionsData.HasFoundCheckpoint,
                ConditionsData.StepCount == ConditionsData.MaxStep,
                ConditionsData.HasTouchedWall
            };
        }

    }
}
