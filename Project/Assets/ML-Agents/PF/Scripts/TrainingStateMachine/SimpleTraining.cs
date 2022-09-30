using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using UnityEngine.Events;
namespace ML_Agents.PF.Scripts.TrainingStateMachine
{
    public class SimpleTraining : TrainingStateMachine
    {

        public SimpleTraining()
        {
        }

        public override void RunStepReward()
        {
            base.RunStepReward();

            if ((int)PhaseType == 1) return;
            if ((int)PhaseType == 4) GiveRewardInternal(RewardUseType.Add_Reward, -0.001f / (MaxStep / 1000)); //3.5
            else GiveRewardInternal(RewardUseType.Add_Reward, -0.0001f / (MaxStep / 10000)); //0.35f
        }

        public override void RunCheckPointReward()
        {
            base.RunCheckPointReward();

            if ((int)PhaseType == 1)
            {
                GiveRewardInternal(RewardUseType.Set_Reward, GameManager.Instance.RewardData.Reward);
                EndEpisode();

                return;
            }

            if ((int)PhaseType == 2)
            {
                GiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward / 2);

                DijkstraDataParsh(_checkPointLength, CP_D_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _checkPointLength))
                {
                    GiveRewardInternal(RewardUseType.Set_Reward, GameManager.Instance.RewardData.Reward);
                }
                EndEpisode();

                return;
            }

            if ((int)PhaseType >= 3)
            {
                GiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward / 3);

                DijkstraDataParsh(_checkPointLength, CP_D_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _checkPointLength))
                {
                    GiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.Reward / 3);
                }

                return;
            }
        }

        public override void RunFinalGoalReward()
        {
            base.RunFinalGoalReward();

            if ((int)PhaseType == 3)
            {
                GiveRewardInternal(RewardUseType.Set_Reward, 1f);
                EndEpisode();

                return;
            }

            if ((int)PhaseType == 4)
            {
                GiveRewardInternal(RewardUseType.Add_Reward, 0.5f);

                DijkstraDataParsh(_pathTotalLength, FULL_D_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _checkPointLength))
                {
                    GiveRewardInternal(RewardUseType.Set_Reward, 1f);
                }
                EndEpisode();

                return;
            }
        }


        // public override void RunHasEpisodeEnded()
        // {
        //     var conditions = new List<bool>
        //     {
        //         _hasFoundCheckpoint,
        //         StepCount == MaxStep,
        //         _hasTouchedWall
        //     };
        //
        // }

    }
}
