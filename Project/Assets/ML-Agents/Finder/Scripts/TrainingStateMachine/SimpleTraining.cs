using System.Collections.Generic;
using UnityEngine.Events;
namespace ML_Agents.Finder.Scripts.TrainingStateMachine
{
    public class SimpleTraining : TrainingStateMachine
    {

        public SimpleTraining(List<UnityEvent> actions) : base(actions)
        {
        }

        public override void RunStepReward()
        {
            base.RunStepReward();
            if ((int)stateMachine.PhaseType == 1) return;
            if ((int)stateMachine.PhaseType == 4) GiveRewardInternal(Use.Add_Reward, -0.001f / ((float)MaxStep / 1000)); //3.5
            else GiveRewardInternal(Use.Add_Reward, -0.0001f / ((float)MaxStep / 10000)); //0.35f
        }

        public override void RunCheckPointReward()
        {
            base.RunCheckPointReward();
            if ((int)stateMachine.PhaseType == 1)
            {
                GiveRewardInternal(Use.Set_Reward, GameManager.Instance.RewardData.Reward);
                EndEpisode();
                return;
            }

            if ((int)stateMachine.PhaseType == 2)
            {
                GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.Reward / 2);

                WriteDijkstraData(_checkPointLength, CP_D_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _checkPointLength))
                {
                    GiveRewardInternal(Use.Set_Reward, GameManager.Instance.RewardData.Reward);
                }
                EndEpisode();
                return;
            }

            if ((int)stateMachine.PhaseType >= 3)
            {
                GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.Reward / 3);

                WriteDijkstraData(_checkPointLength, CP_D_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _checkPointLength))
                {
                    GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.Reward / 3);
                }
                return;
            }
        }

        public override void RunFinalGoalReward()
        {
            base.RunFinalGoalReward();

            if ((int)stateMachine.PhaseType == 3)
            {
                GiveRewardInternal(Use.Set_Reward, 1f);
                EndEpisode();

                return;
            }

            if ((int)stateMachine.PhaseType == 4)
            {
                GiveRewardInternal(Use.Add_Reward, 0.5f);

                WriteDijkstraData(_pathTotalLength, FULL_D_KEY);

                if (Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _checkPointLength))
                {
                    GiveRewardInternal(Use.Set_Reward, 1f);
                }
                EndEpisode();

                return;
            }




        public override void RunHasEpisodeEnded()
        {
            conditions = new List<bool>
            {
                _hasFoundCheckpoint,
                StepCount == MaxStep,
                _hasTouchedWall
            };

        }

    }
}
