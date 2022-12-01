using UnityEngine;

namespace ML_Agents.PF.Scripts.Data
{
    [CreateAssetMenu(fileName = "RewardData", menuName = "Data/RewardData", order = 0)]
    public class RewardData : ScriptableObject
    {

        /// <summary>
        /// Holds the data related to the reward system
        /// </summary>

        [Header("Reward Value")]
        [Range(0, 10)]
        public float Reward = 1;

        [Header("Step Rewards")]
        [Range(0, 1)]
        public float StepReward = 1;
        [Range(0, 1)]
        public float StepPenaltyPerSec = 1;

        public int DivRewardValue = 10;

        [Range(0f, 1f)]
        public float Epsilon = 0.4f;

        [Range(0.001f, 0.1f)][Tooltip("Higher value => less rewards/step")]
        public float StepRewardFrequency = 0.005f;

        [Space]
        [Header("Penalty Rewards")]
        [Range(0, -2)]
        public float Penalty = -1;

        [Space]
        [Header("Extra Params")]
        [Range(0, 6)]
        public int ExtraDistance = 2;
    }
}
