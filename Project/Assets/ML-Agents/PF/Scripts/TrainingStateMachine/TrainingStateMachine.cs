using System.Collections.Generic;
using ML_Agents.PF.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace ML_Agents.PF.Scripts.TrainingStateMachine
{
    public class TrainingStateMachine
    {
        // private readonly List<UnityEvent> _actions;

        public PhaseType PhaseType;
        public TrainingType TrainingType;

        public float MaxStep;

        protected int _traveledDistance;
        protected int _checkPointLength;
        protected int _pathTotalLength;

        public const string CP_D_KEY = "Agent/Check Point Dijkstra Success Rate";
        public const string FULL_D_KEY = "Agent/Full Dijkstra Success Rate";



        public TrainingStateMachine(PhaseType phaseType, TrainingType trainingType)
        {
            PhaseType = phaseType;
            TrainingType = trainingType;
        }

        public TrainingStateMachine()
        {
            //List<UnityEvent> actions
            // _actions = actions;
        }



        public virtual void RunStepReward()
        {

            if (RunHasEpisodeEnded())
            {
                CalculateReward();
                // Debug.Log("Episode Ended | Calculated reward : " + CalculateReward());
                GiveRewardInternal(RewardUseType.Add_Reward, CalculateReward());
                EndEpisode();
            }

            // foreach (var action in _actions)
            // {
            //     action?.Invoke();
            // }
        }

        public virtual void RunCheckPointReward()
        {

        }

        public virtual void RunFinalGoalReward()
        {

        }



        public virtual void RunHarmfulCollision()
        {
            // if (GameManager.Instance._stateMachine.PhaseType != GameManager.PhaseType.Phase_A)
            // {
            //     GiveRewardInternal(UxmlAttributeDescription.Use.Add_Reward, GameManager.Instance.RewardData.WallPenalty / 2);
            //
            //     return;
            // }
            // GiveRewardInternal(UxmlAttributeDescription.Use.Set_Reward, GameManager.Instance.RewardData.WallPenalty);
        }

        public virtual bool RunHasEpisodeEnded()
        {

            return true;
        }

        public float CalculateReward()
        {
            return 1;
        }

        public void RunCalculateReward()
        {
            // var conditions = GameManager.Instance._stateMachine.PhaseType == GameManager.PhaseType.Phase_D ?
            //     new List<bool>()
            //     {
            //         Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _pathTotalLength) && _hasFoundGoal && _hasFoundCheckpoint,
            //         _hasFoundGoal,
            //         _hasFoundCheckpoint,
            //     }
            //     : new List<bool>()
            //     {
            //         Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _pathTotalLength) && _hasFoundCheckpoint,
            //         _hasFoundCheckpoint,
            //     };
        }



        public void DijkstraDataParsh(int length, string key)
        {
            Utils.Utils.WriteDijkstraData(_traveledDistance, length, key);
        }

        public void GiveRewardInternal(RewardUseType rewardUseType , float reward)
        {

        }

        public void EndEpisode()
        {

        }
    }
}
