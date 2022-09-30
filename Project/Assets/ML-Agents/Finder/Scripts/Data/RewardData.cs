using UnityEngine;

namespace ML_Agents.Finder.Scripts
{
    [CreateAssetMenu(fileName = "RewardData", menuName = "RL/RewardData", order = 0)]
    public class RewardData : ScriptableObject
    {

        [Header("Reward Values")]
        public float Reward = 1;

        [Header("Step Rewards")]
        [Range(0, 1)]
        public float StepReward = 1;
        public float Epsilon = 0.3f;

        [Header("Penalty Rewards")]
        [Range(0, -1)]
        public float WallPenalty = 10;
        public float FoundGoalReward = 10;

        [Header("Extra Params")]
        [Range(0,6)]
        public int ExtraDistance = 2;
    }
}
