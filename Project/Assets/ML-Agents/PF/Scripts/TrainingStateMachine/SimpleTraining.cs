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
                GiveInternalReward(RewardUseType.Add_Reward, reward / 1000f);  //0.1f
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                GiveInternalReward(RewardUseType.Add_Reward, reward / 100f); //0.01f
            }

        } //done

        public override void RunOnCheckPointReward() //done
        {
            base.RunOnCheckPointReward();

            if (PhaseType == PhaseType.Phase_A)
            {
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
                EpisodeEndCallBack();
            }
            else
            {
                if (PhaseType == PhaseType.Phase_B)
                {
                    GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward / 2);
                    EpisodeEndCallBack();
                }
                else if (PhaseType == PhaseType.Phase_C || PhaseType == PhaseType.Phase_D)
                {
                    GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward / 3);
                }

                DijkstraDataWriter(ConditionsData.CheckPointPathLength, CHECK_POINT_KEY);
                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength))
                {
                    Debug.Log($"Current Length : {ConditionsData.TraveledDistance} / {ConditionsData.CheckPointPathLength}" );
                    Debug.Log($"#Simple Machine# -> Half dijkstra success ");
                    GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
                }
            }
        }

        public override void RunOnFinalGoalReward() //done
        {
            base.RunOnFinalGoalReward();

            if (PhaseType == PhaseType.Phase_C)
            {
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Reward);
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward/2);

                DijkstraDataWriter(ConditionsData.FullPathLength, FINAL_GOAL_KEY);
                Debug.Log($"Current Length : {ConditionsData.TraveledDistance} / {ConditionsData.CheckPointPathLength} + {ConditionsData.FullPathLength}" );
                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength + ConditionsData.FullPathLength))
                {
                    Debug.Log($"#Simple Machine# -> Full dijkstra success ");
                    GiveInternalReward(RewardUseType.Set_Reward, 1f);
                }
            }

            EpisodeEndCallBack();
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
