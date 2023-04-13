using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.RL;
using ML_Agents.PF.Scripts.Structs;
using ML_Agents.PF.Scripts.UtilsScripts;
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

        protected const string CHECK_POINT_KEY = "Agent/Half Dijkstra Success Rate";
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
        public UnityAction RevisitedCallBack;

        public bool IsOutOfTime;

        protected float PreviousStepReward;

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

            if (IsOutOfTime)//give a very small penalty if the agent is in the same node for too long
            {
                Debug.Log("out of time");
                GiveInternalReward(RewardUseType.Add_Reward, ((RewardData.StepPenalty / ConditionsData.MaxStep)* RewardData.TimePenaltyMultiplier)*-1f);
            }
        }

        public virtual void RunOnCheckPointReward()
        {
            UpdateEndEpisodeConditionsList();
            UpdateRewardConditionsList();

            if ((int)PhaseType >= 3)
            {
                PreviousStepReward = 0;
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
                GiveInternalReward(RewardUseType.Add_Reward, RewardData.Penalty/10);
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

        protected void OnTerminalCondition()
        {
            //this will update the Reward Data struct with the final reward condition
            UpdateRewardDataStructCallBack?.Invoke();
            var reward = RewardFunction.GetComplexReward(RewardDataStruct);
            GiveInternalReward(RewardUseType.Set_Reward, reward);
            EndEpisode();
        }

        protected void GiveInternalReward(RewardUseType rewardUseType, float reward)
        {
            Debug.Log($"used reward of type {rewardUseType} with value : {reward}");
            GiveInternalRewardCallBack?.Invoke(rewardUseType, reward);
        }

        protected void DijkstraDataWriter(string key, bool result)
        {
            Utils.WriteDijkstraData(key, result);
        }

    #endregion

        public void OnRevisitedNode(bool result)
        {
            ConditionsData.HasRevisitedNode = result;

            if(!result) return;

            //if it out of time and resets timer by moving to next node, add points
            if (IsOutOfTime) GiveInternalReward(RewardUseType.Add_Reward, RewardData.Reward/5f);
            if(!RewardData.UseRevisitPenalty) return;

            GiveInternalReward(RewardUseType.Add_Reward, RewardData.RevisitNodePenalty);
        }

        public bool HasEpisodeEnded()
        {
            RewardDataStruct.HasEpisodeEnd = Utils.HasEpisodeEnded(EndEpisodeConditions);

            return RewardDataStruct.HasEpisodeEnd;
        }

        protected void DijkstraReward(RewardUseType type, int pathLength, string key)
        {
            //create dijkstra analytics
            var result = Utils.IsCurrDistLessThanPathLength(ConditionsData.TraveledDistance, pathLength, false);
            if (result)
            {
                GiveInternalReward(type, RewardData.Reward);
            }
            DijkstraDataWriter(key, result);
        }

    }
}
