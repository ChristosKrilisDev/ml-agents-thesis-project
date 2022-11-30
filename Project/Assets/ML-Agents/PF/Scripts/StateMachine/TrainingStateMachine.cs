using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.RL;
using ML_Agents.PF.Scripts.Structs;
using ML_Agents.PF.Scripts.UtilsScripts;
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

        // end conditions will be used for EndEpisode check
        protected abstract void UpdateEndEpisodeConditionsList();
        public List<bool> EndEpisodeConditions = new List<bool>();

        //Reward conditions must ne used once the episode ends to calculate the final reward
        public List<bool> FinalRewardConditions;
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

            UpdateRewardConditionsList();
        }

        private void UpdateRewardConditionsList()
        {
            var cndList = new List<bool>();

            if (PhaseType == PhaseType.Phase_A)
            {
                cndList.Add(ConditionsData.HasFoundCheckpoint);
            }
            else if (PhaseType == PhaseType.Phase_B)
            {
                cndList.Add(Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength, false) && ConditionsData.HasFoundCheckpoint);
                cndList.Add(ConditionsData.HasFoundCheckpoint);
            }
            else if (PhaseType == PhaseType.Phase_C)
            {
                cndList.Add(ConditionsData.HasFoundGoal);
                cndList.Add(Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.CheckPointPathLength, false) && ConditionsData.HasFoundCheckpoint);
                cndList.Add(ConditionsData.HasFoundGoal);
            }
            else if (PhaseType == PhaseType.Phase_D)
            {
                cndList.Add(Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, ConditionsData.FullPathLength, false) && ConditionsData.HasFoundGoal);
                cndList.Add(ConditionsData.HasFoundGoal);
                cndList.Add(ConditionsData.HasFoundGoal);
            }

            FinalRewardConditions = cndList;
        }

    #endregion

    #region Virtual Methods

        public virtual void RunOnStepReward()
        {
            UpdateEndEpisodeConditionsList();
            UpdateRewardConditionsList();
            //step factor multiplies the final reward
            ConditionsData.StepFactor = (ConditionsData.MaxStep - ConditionsData.StepCount) / (float)ConditionsData.MaxStep;
        }

        public virtual void RunOnCheckPointReward()
        {
            UpdateEndEpisodeConditionsList();
            UpdateRewardConditionsList();

            if ((int)PhaseType >= 3)
            {
                SwitchTargetNodeCallBack?.Invoke();
            }
        }

        public virtual void RunOnFinalGoalReward()
        {
            UpdateEndEpisodeConditionsList();
            UpdateRewardConditionsList();
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

            return RewardFunction.GetComplexReward(RewardDataStruct);
        }

        protected void OnTerminalCondition(RewardUseType rType)
        {
            //this will update the Reward Data struct with the final reward condition
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
            Utils.WriteDijkstraData(ConditionsData.TraveledDistance, length, key);
        }

    #endregion

        public bool HasEpisodeEnded()
        {
            RewardDataStruct.HasEpisodeEnd = Utils.HasEpisodeEnded(EndEpisodeConditions);

            return RewardDataStruct.HasEpisodeEnd;
        }

    }
}
