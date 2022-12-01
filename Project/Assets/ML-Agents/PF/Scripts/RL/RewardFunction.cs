using System.Collections.Generic;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Structs;
using UnityEngine;

namespace ML_Agents.PF.Scripts.RL
{
    public static class RewardFunction
    {
        private static RewardData RewardData => GameManager.Instance.RewardData;

        //TODO : Avoiding Reward Hacking | Avoiding Side Effects | Scalable Oversight | Safe Exploration | Robustness to Distributional Shift

        /// <SharpedRewardFunction> distanceReward = 1 - ( Dx/DijDχ )^(epsilon) </SharpedRewardFunction>
        /// <distanceReward> The value of the reward based on how close to the target the agent is </distanceReward>
        /// <Dx> the distance between agent and goal in strait line from point A to B </Dx>
        /// <DijDx> the min distance (on init) from player pos, to goal </DijDx>
        /// <epsilon> value to create sharped gradient learning curve </epsilon>
        /// <maxBonusReward> A huge reward given to the agent when it completes the task </maxBonusReward>
        ///
        /// <currDistance> The current distance between agent and goal</currDistance>
        /// <hasEpisodeEnded> Whenever the episodes ends, calculate terminal reward </hasEpisodeEnded>
        ///
        /// <calculateReward> The reward agent will take for that state based based on the reward state</calculateReward>
        public static float GetComplexReward(RewardDataStruct rewardDataStruct)
        {
            float calculateReward;

            if (!rewardDataStruct.HasEpisodeEnd)
            {
                calculateReward = GetStepRewardReward(rewardDataStruct.CurrentDistance, rewardDataStruct.InitialDistanceFromTarget);
                return calculateReward;
            }

            CreateConditionsList(rewardDataStruct, out var conditions);
            calculateReward = GetTerminalConditionReward(conditions);

            return calculateReward;
        }

        private static void CreateConditionsList(RewardDataStruct rewardDataStruct, out List<bool> conditions)
        {
            conditions = new List<bool>();

            foreach (var condition in rewardDataStruct.Conditions)
            {
                conditions.Add(condition);
            }
            conditions.Add(true);
        }

        private static float GetStepRewardReward(float currentDistance, float distanceFromTarget)
        {
            //encourage agent to keep searching, distance based
            var distanceReward = 1 - Mathf.Pow(currentDistance / distanceFromTarget, RewardData.Epsilon);

            return distanceReward;
        }

        private static float GetTerminalConditionReward(List<bool> conditions)
        {
            float reward = 0;
            for (var i = 0; i < conditions.Count; i++)
            {
                //conditions are reversed ordered,
                //C1 Full success, C2 Partial success, C3 Partial success
                if (!conditions[i]) continue;

                //C1 => r, C2 => r/2, C3 => r/3
                reward = RewardData.Reward / (i+1);

                Debug.Log("Terminal reward = " + reward + " takes  : " +i);
                break;
            }

            return reward;
        }

    }
}
