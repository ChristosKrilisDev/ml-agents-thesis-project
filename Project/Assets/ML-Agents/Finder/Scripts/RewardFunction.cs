using System;
using UnityEngine;

namespace ML_Agents.Finder.Scripts
{
    public static class RewardFunction
    {

        [Header("Reward Function Vars")]
        public static int MaxStep;

        private const float EPSILON = 0.7f;
        private const float BOOST_REWARD = 1;

        //TODO : Avoiding Reward Hacking | Avoiding Side Effects | Scalable Oversight | Safe Exploration | Robustness to Distributional Shift
        //TODO : Sharped RF

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
        public static float GetComplexReward(float currDistance, float currentGoalDistance, int stepCount, bool endEpisode, bool hasFoundCheckPoint, bool hasFoundGoal, bool hasFoundShortestPath)
        {
            // Additionally, the magnitude of the reward should not exceed 1.0
            //TODO : Make this with less params
            float calculateReward;

            if (!endEpisode)//TC1 when it ends? : max step reached or completed task
            {
                //encourage agent to keep searching, distance based
                var distanceReward = 1 - Mathf.Pow(currDistance / currentGoalDistance, EPSILON);
                calculateReward = distanceReward;

                //50% less //reward a very small amount, to guide the agent but not big enough to create a looped reward(circle).
                return calculateReward;
            }


        #region TerminalRewards

            if(!hasFoundCheckPoint) calculateReward = -BOOST_REWARD; // -1 worst scenario
            if(!hasFoundGoal) calculateReward = -BOOST_REWARD * 3; //-0.5f
            if(!hasFoundShortestPath) calculateReward = -BOOST_REWARD / 4; //-0.25f


            var stepFactor = Math.Abs(stepCount - MaxStep) / (float)MaxStep; //TODO : remove this, get it from agent
            calculateReward = Math.Abs(BOOST_REWARD + stepFactor); //1 + SF

        #endregion

            return calculateReward;
        }

        public static float GetComplexReward(float currDistance, float currentGoalDistance, int stepCount, bool endEpisode, bool hasFoundCheckPoint, bool hasFoundShortestPath)
        {
            float calculateReward;

            if (!endEpisode)//TC1 when it ends? : max step reached or completed task
            {
                //encourage agent to keep searching, distance based
                var distanceReward = 1 - Mathf.Pow(currDistance / currentGoalDistance, EPSILON);
                calculateReward = distanceReward;

                //50% less //reward a very small amount, to guide the agent but not big enough to create a looped reward(circle).
                return calculateReward;
            }

        #region TerminalRewards

            if(!hasFoundCheckPoint) calculateReward = -BOOST_REWARD; // -1 worst scenario
            if(!hasFoundShortestPath) calculateReward = -BOOST_REWARD / 4; //-0.25f

            var stepFactor = Math.Abs(stepCount - MaxStep) / (float)MaxStep;
            calculateReward = Math.Abs(BOOST_REWARD + stepFactor); //1 + SF

        #endregion

            return calculateReward;
        }
    }

}
