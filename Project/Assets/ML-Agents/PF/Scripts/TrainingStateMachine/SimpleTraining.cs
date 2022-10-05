using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;

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
            if (PhaseType == PhaseType.Phase_D)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, (-RewardData.StepPenaltyPerSec * 10 ) / ((float)ConditionsData.MaxStep / 1000));
            }
            else //B and C
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, -RewardData.StepPenaltyPerSec / ((float)ConditionsData.MaxStep / 10000));
            }
        }

        public override void RunOnCheckPointReward()
        {
            base.RunOnCheckPointReward();
            if (PhaseType == PhaseType.Phase_A)
            {
                RunGiveRewardInternal(RewardUseType.Set_Reward, GameManager.Instance.RewardData.Reward);
                RunEndEpisode();
            }
            else if (PhaseType == PhaseType.Phase_B) //use dijkstra
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward / 2);

                RunDijkstraDataWriter(ConditionsData.CheckPointLength, CHECK_POINT_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointLength))
                {
                    RunGiveRewardInternal(RewardUseType.Set_Reward, GameManager.Instance.RewardData.Reward);
                }
                RunEndEpisode();
            }
            else if (PhaseType == PhaseType.Phase_C || PhaseType == PhaseType.Phase_D)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward / 3);
                RunDijkstraDataWriter(ConditionsData.CheckPointLength, CHECK_POINT_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointLength))
                {
                    RunGiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward / 3);
                }
                // SwitchTargetNodeCallBack?.Invoke();
            }
        }

        public override void RunOnFinalGoalReward()
        {
            base.RunOnFinalGoalReward();

            if (PhaseType == PhaseType.Phase_C)
            {
                RunGiveRewardInternal(RewardUseType.Set_Reward, RewardData.Reward);
                RunEndEpisode();

            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, 0.5f);

                RunDijkstraDataWriter(ConditionsData.PathTotalLength, FINAL_GOAL_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.CheckPointLength))
                {
                    RunGiveRewardInternal(RewardUseType.Set_Reward, 1f);
                }
                RunEndEpisode();
            }
        }

        protected override List<bool> CreateEndConditionsList()
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
