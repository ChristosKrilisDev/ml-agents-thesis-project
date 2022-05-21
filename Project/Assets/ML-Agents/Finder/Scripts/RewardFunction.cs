﻿using System;
using UnityEngine;


namespace ML_Agents.Finder.Scripts
{
    public static class RewardFunction
    {

        [Header("Reward Function Vars")]
        public static int MaxStep;

        private const float EPSILON = 0.4f;
        private const float BOOST_REWARD = 1;

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
        public static float GetComplexReward(float currDistance, float currentGoalDistance, int stepCount, bool hasEpisodeEnded, bool hasFoundCheckPoint, bool hasFoundGoal, bool hasFollowedDijkstra)
        {
            float calculateReward ;
            //agent reward is reset to 0 after every step
            // Additionally, the magnitude of the reward should not exceed 1.0
            var stepFactor = Math.Abs(stepCount - MaxStep) / (float)MaxStep;

            #region TerminalRewards

            if (hasEpisodeEnded) //TC1 when it ends? : max step reached or completed task
            {
                if (hasFoundCheckPoint)
                {
                    if (hasFoundGoal)
                    {
                        if (hasFollowedDijkstra)
                        {
                            calculateReward = Math.Abs(BOOST_REWARD + stepFactor); //1 + SF
                            //Debug.Log($"Phase : ALl true \t reward : {calculateReward}");
                        }
                        else
                        {
                            calculateReward = -BOOST_REWARD / 4; //-0.25f
                            //Debug.Log($"Phase : Distance is more than Dijkstra * 2 \t reward : " + calculateReward);
                        }
                    }
                    else
                    {
                        calculateReward = -BOOST_REWARD * 3; //-0.5f
                        //Debug.Log($"Phase : Didnt find goal \t reward :  {calculateReward}");
                    }
                }
                else
                {
                    calculateReward = -BOOST_REWARD * 4; // -1 worst scenario
                    //Debug.Log($"Phase : Didnt CP goal \t reward :  {calculateReward}");
                }
                
            }

            #endregion
            else //encourage agent to keep searching
            {
                var distanceReward = 1 - Mathf.Pow(currDistance / currentGoalDistance, EPSILON);

                //TODO : 0/100 ??!!
                calculateReward = distanceReward;
                //50% less //reward a very small amount, to guide the agent but not big enough to create a looped reward(circle).
                //Debug.LogFormat("Phase : Encourage \t reward : {0}  | target {1}", calculateReward, goalDistances[goalIndex]);
            }

            //TODO : DEBUgg
            //Debug.Log($"Calculated reward = {calculateReward}");
            return calculateReward;
            //Use AddReward() to accumulate rewards between decisions.
            //Use SetReward() to overwrite any previous rewards accumulate between decisions.
        }
    }
}
