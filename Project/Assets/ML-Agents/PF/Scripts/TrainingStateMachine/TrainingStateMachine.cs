using System.Collections.Generic;
using UnityEngine.Events;
namespace ML_Agents.PF.Scripts.TrainingStateMachine
{
    public class TrainingStateMachine
    {
        private readonly List<UnityEvent> _actions;

        public TrainingStateMachine(List<UnityEvent> actions)
        {
            _actions = actions;
        }



        public virtual void RunStepReward()
        {

            if (RunHasEpisodeEnded())
            {
                CalculateReward();
                Debug.Log("Episode Ended | Calculated reward : " + CalculateReward());
                GiveRewardInternal(Use.Add_Reward, CalculateReward());
                EndEpisode();
            }

            foreach (var action in _actions)
            {
                action?.Invoke();
            }
        }

        public virtual void RunCheckPointReward()
        {

        }

        public virtual void RunFinalGoalReward()
        {

        }

        public virtual void RunHarmfulCollision()
        {
            if (GameManager.Instance._stateMachine.PhaseType != GameManager.PhaseType.Phase_A)
            {
                GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.WallPenalty / 2);

                return;
            }
            GiveRewardInternal(Use.Set_Reward, GameManager.Instance.RewardData.WallPenalty);
        }

        public virtual void RunHasEpisodeEnded()
        {

        }

        public void RunCalculateReward()
        {
            var conditions = GameManager.Instance._stateMachine.PhaseType == GameManager.PhaseType.Phase_D ?
                new List<bool>()
                {
                    Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _pathTotalLength) && _hasFoundGoal && _hasFoundCheckpoint,
                    _hasFoundGoal,
                    _hasFoundCheckpoint,
                }
                : new List<bool>()
                {
                    Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _pathTotalLength) && _hasFoundCheckpoint,
                    _hasFoundCheckpoint,
                };
        }
    }
}
