﻿using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.RL;
using ML_Agents.PF.Scripts.Structs;
using UnityEngine;
using UnityEngine.Events;

namespace ML_Agents.PF.Scripts.TrainingStateMachine
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
        protected List<bool> HasEndConditions = new List<bool>();

        public readonly List<bool> RewardConditions;
        public RewardDataStruct RewardDataStruct;

        public UnityAction<RewardUseType, float> GiveInternalRewardCallBack;
        public UnityAction EndEpisodeCallBack;
        public UnityAction SwitchTargetNodeCallBack;
        public UnityAction UpdateRewardDataWrapperCallBack;


    #endregion

    #region Constractors

        protected TrainingStateMachine(PhaseType phaseType, TrainingType trainingType)
        {
            PhaseType = phaseType;
            TrainingType = trainingType;
            ConditionsData = new ConditionsData();

            // Debug.Log("State Machine created");
            RewardConditions = CreateRewardConditionsList();
        }

        private List<bool> CreateRewardConditionsList()
        {
            if (PhaseType == PhaseType.Phase_D)
            {
                return new List<bool>()
                {
                    Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.FullPathLength) && ConditionsData.HasFoundGoal && ConditionsData.HasFoundCheckpoint,
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
            //TODO : error -> infinite reward value
            // if (PhaseType == PhaseType.Phase_C)
            // {
            //     //give a negative reward each time agent makes an action
            //     ConditionsData.StepFactor = (ConditionsData.StepFactor - ConditionsData.MaxStep) / ConditionsData.MaxStep; //current step?
            //     RunGiveRewardInternal(RewardUseType.Add_Reward, - ConditionsData.StepFactor);
            // }

            if (HasEpisodeEnded())
            {
                CalculateComplexRewardCallBack();
                EpisodeEndCallBack();
            }
        }

        public virtual void RunOnCheckPointReward()
        {
            // Debug.Log("#State Machine# Check Point Found...");

            if ((int)PhaseType >= 3)
            {
                SwitchTargetNodeCallBack?.Invoke();
            }
        }

        public virtual void RunOnFinalGoalReward()
        {
            // Debug.Log("#State Machine# Final Point Found...");

        }

        public virtual void RunOnHarmfulCollision()
        {
            if (PhaseType == PhaseType.Phase_A)
            {
                // Debug.Log("#State Machine# Hit Wall <dead>...");
                GiveRewardInternalCallBack(RewardUseType.Set_Reward, RewardData.Penalty);
                EpisodeEndCallBack();
            }
            else
            {
                // Debug.Log("#State Machine# Hit Wall...");
                GiveRewardInternalCallBack(RewardUseType.Add_Reward, RewardData.Penalty / 2);
            }
        }

        protected virtual void EpisodeEndCallBack()
        {
            Debug.Log("#State Machine# Episode ended");
            EndEpisodeCallBack?.Invoke();
        }

    #endregion

    #region Protected Methods CallBacks

        protected float CalculateComplexRewardCallBack()
        {
            UpdateRewardDataWrapperCallBack?.Invoke();
            Debug.Log("#State Machine# Complex Reward :" + RewardFunction.GetComplexReward(RewardDataStruct));
            return RewardFunction.GetComplexReward(RewardDataStruct);
        }

        protected void OnTerminalConditionCallBack(RewardUseType rType)
        {
            EpisodeEndCallBack();
        }

        protected void GiveRewardInternalCallBack(RewardUseType rewardUseType, float reward)
        {
            // Debug.Log($"#State Machine# -> {rewardUseType} reward internal : {reward} ");

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
