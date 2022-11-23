using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.RL;
using ML_Agents.PF.Scripts.Structs;
using UnityEngine;
using UnityEngine.Events;

namespace ML_Agents.PF.Scripts.StateMachine
{
    public abstract class TrainingStateMachine
    {

    #region Vars

        protected static RewardData RewardData => GameManager.Instance.RewardData;

        protected readonly PhaseType PhaseType;
        protected readonly TrainingType TrainingType;
        public readonly ConditionsData ConditionsData;

        protected const string CHECK_POINT_KEY = "Agent/Check Point Dijkstra Success Rate";
        protected const string FINAL_GOAL_KEY = "Agent/Full Dijkstra Success Rate";

        protected abstract List<bool> CreateEndConditionsList();
        protected List<bool> HasEndConditions = new List<bool>(); //?

        public readonly List<bool> RewardConditions;
        public RewardDataStruct RewardDataStruct;

        public UnityAction<RewardUseType, float> GiveInternalRewardCallBack;
        public UnityAction EndEpisodeCallBack;
        public UnityAction SwitchTargetNodeCallBack;
        public UnityAction UpdateRewardDataStructCallBack;

    #endregion

    #region Constractors

        protected TrainingStateMachine(PhaseType phaseType, TrainingType trainingType)
        {
            PhaseType = phaseType;
            TrainingType = trainingType;
            ConditionsData = new ConditionsData();

            RewardConditions = CreateRewardConditionsList();
        }

        private List<bool> CreateRewardConditionsList()
        {
            if (PhaseType == PhaseType.Phase_D)
            {
                return new List<bool>()
                {
                    Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.FullPathLength) && ConditionsData.HasFoundGoal,
                    ConditionsData.HasFoundGoal,
                    ConditionsData.HasFoundCheckpoint,
                };
            }

            return new List<bool>()
            {
                Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.FullPathLength) && ConditionsData.HasFoundCheckpoint,
                ConditionsData.HasFoundCheckpoint,
            };
        }

    #endregion

    #region Virtual Methods

        public virtual void RunOnStepReward()
        {
            //step factor multiplies the final reward
            ConditionsData.StepFactor = (ConditionsData.MaxStep - ConditionsData.StepCount) / (float)ConditionsData.MaxStep;

            Debug.Log($"RunOnStepReward from Parent Class");
        }

        public virtual void RunOnCheckPointReward()
        {
            if ((int)PhaseType >= 3)
            {
                SwitchTargetNodeCallBack?.Invoke();
            }
        }

        public virtual void RunOnFinalGoalReward()
        {

        }

        public void RunOnHarmfulCollision()
        {
            if (PhaseType == PhaseType.Phase_A)
            {
                GiveInternalReward(RewardUseType.Set_Reward, RewardData.Penalty);
                EndEpisode();
            }
            else
            {
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Penalty / 2);
            }
        }

        protected virtual void EndEpisode()
        {
            EndEpisodeCallBack?.Invoke();
        }

    #endregion

    #region Protected Methods CallBacks

        protected float CalculateComplexReward()
        {
            UpdateRewardDataStructCallBack?.Invoke();

            // Debug.Log("#State Machine# Complex Reward :" + RewardFunction.GetComplexReward(RewardDataStruct));
            return RewardFunction.GetComplexReward(RewardDataStruct);
        }

        protected void OnTerminalCondition(RewardUseType rType)
        {
            UpdateRewardDataStructCallBack?.Invoke();
            var reward = RewardFunction.GetComplexReward(RewardDataStruct);
            GiveInternalReward(rType, reward);
            EndEpisode();
        }

        protected void GiveInternalReward(RewardUseType rewardUseType, float reward)
        {
            GiveInternalRewardCallBack?.Invoke(rewardUseType, reward);
        }

        protected void DijkstraDataWriter(int length, string key)
        {
            Utils.Utils.WriteDijkstraData(ConditionsData.TraveledDistance, length, key);
        }

    #endregion

        public bool HasEpisodeEnded()
        {
            return Utils.Utils.HasEpisodeEnded(RewardConditions);
        }

    }
}
