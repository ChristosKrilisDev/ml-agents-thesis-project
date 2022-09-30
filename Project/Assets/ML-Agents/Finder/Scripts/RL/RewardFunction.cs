﻿using System.Collections.Generic;
using ML_Agents.Finder.Scripts.Structs;
using UnityEngine;
namespace ML_Agents.Finder.Scripts.RL
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
        public static float GetComplexReward(RewardDataWrapper rewardDataWrapper)
        {
            float calculateReward;

            if (!rewardDataWrapper.HasEpisodeEnd)
            {
                calculateReward = GetStepRewardReward(rewardDataWrapper.CurrentDistance, rewardDataWrapper.CurrentTargetDistance);
                return calculateReward;
            }

            CreateConditionsList(rewardDataWrapper, out var conditions);
            calculateReward = GetTerminalConditionReward(conditions);

            return calculateReward;
        }


        private static void CreateConditionsList(RewardDataWrapper rewardDataWrapper, out List<bool> conditions)
        {
            conditions = new List<bool>();
            foreach (var condition in rewardDataWrapper.Conditions)
            {
                conditions.Add(condition);
            }
            conditions.Add(true);
        }

        private static float GetStepRewardReward(float cDistance , float cTargetDistance)
        {
            //encourage agent to keep searching, distance based
            var distanceReward = 1 - Mathf.Pow(cDistance / cTargetDistance, RewardData.Epsilon);

            return distanceReward;
        }

        private static float GetTerminalConditionReward(List<bool> conditions)
        {
            float reward = 0;

            for (var i = 0; i < conditions.Count; i++)
            {
                if (conditions[i]) continue;

                reward = RewardData.Reward/i; //1st => 1, 2nd => 0.5, 3rd => 0.25f
                break;
            }

            return reward;
        }

    }
}