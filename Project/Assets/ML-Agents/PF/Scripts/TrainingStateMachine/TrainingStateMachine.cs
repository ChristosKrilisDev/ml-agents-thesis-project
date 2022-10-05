using System.Collections.Generic;
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

        public UnityAction EndEpisodeCallBack;
        public UnityAction<RewardUseType, float> GiveInternalRewardCallBack;
        public UnityAction SwitchTargetNodeCallBack;
        public UnityAction UpdateRewardDataWrapperCallBack;


    #endregion

    #region Constractors

        protected TrainingStateMachine(PhaseType phaseType, TrainingType trainingType)
        {
            PhaseType = phaseType;
            TrainingType = trainingType;
            ConditionsData = new ConditionsData();

            Debug.Log("State Machine");
            RewardConditions = CreateRewardConditionsList();
        }

        private List<bool> CreateRewardConditionsList()
        {
            if (PhaseType == PhaseType.Phase_D)
            {
                return new List<bool>()
                {
                    Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.PathTotalLength) && ConditionsData.HasFoundGoal && ConditionsData.HasFoundCheckpoint,
                    ConditionsData.HasFoundGoal,
                    ConditionsData.HasFoundCheckpoint,
                };
            }

            return new List<bool>()
            {
                Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(ConditionsData.TraveledDistance, ConditionsData.PathTotalLength) && ConditionsData.HasFoundCheckpoint,
                ConditionsData.HasFoundCheckpoint,
            };
        }

    #endregion

    #region Virtual Methods

        //class specific
        public virtual void RunOnStepReward()
        {
            if (PhaseType == PhaseType.Phase_C) //valideate this
            {
                //give a negative reward each time agent makes an action
                ConditionsData.StepFactor = (ConditionsData.StepFactor - ConditionsData.MaxStep) / ConditionsData.MaxStep; //current step?
                RunGiveRewardInternal(RewardUseType.Add_Reward, - ConditionsData.StepFactor);
            }

            if (!HasEpisodeEnded()) return;

            RunCalculateComplexReward();
            RunEndEpisode();
        }

        public virtual void RunOnCheckPointReward()
        {
            Debug.Log("Check point done");

            if ((int)PhaseType >= 3)
            {
                SwitchTargetNodeCallBack?.Invoke();
            }
        }

        public virtual void RunOnFinalGoalReward()
        {
            Debug.Log("Check point done");

        }

        public void RunOnHarmfulCollision()
        {
            if (PhaseType != PhaseType.Phase_A)
            {
                RunGiveRewardInternal(RewardUseType.Add_Reward, GameManager.Instance.RewardData.WallPenalty / 2);
            }
            else
            {
                RunGiveRewardInternal(RewardUseType.Set_Reward, GameManager.Instance.RewardData.WallPenalty);
                HasEpisodeEnded();
            }
        }

        protected virtual void RunOnTerminalCondition(RewardUseType rType)
        {

        }



    #endregion

    #region Protected Methods CallBacks

        protected float RunCalculateComplexReward()
        {
            UpdateRewardDataWrapperCallBack?.Invoke();
            return RewardFunction.GetComplexReward(RewardDataStruct);
        }

        protected void RunGiveRewardInternal(RewardUseType rewardUseType, float reward)
        {
            GiveInternalRewardCallBack?.Invoke(rewardUseType, reward);
        }

        protected void RunEndEpisode()
        {
            EndEpisodeCallBack?.Invoke();
        }

        protected void RunDijkstraDataWriter(int length, string key)
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
