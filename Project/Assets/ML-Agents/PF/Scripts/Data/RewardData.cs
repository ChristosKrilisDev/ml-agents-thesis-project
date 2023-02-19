using UnityEngine;

namespace ML_Agents.PF.Scripts.Data
{
    [CreateAssetMenu(fileName = "RewardData", menuName = "Data/RewardData", order = 0)]
    public class RewardData : ScriptableObject
    {

        /// <summary>
        /// Holds the data related to the reward system
        /// </summary>

        [Header("Timer")]
        [Range(1f, 10f)]
        public float TimerValue = 6;

        [Header("Reward Value")]
        [Range(1, 10)]
        public int Reward = 1;

        [Header("Penalty Rewards")]
        [Range(0, -2)]
        public float Penalty = -1;

        [Space]
        [Header("Step Rewards")]
        [Range(-3f, 0), Tooltip("The max penalty reward")]
        public float StepPenalty = -1.5f;

        [Space]
        [Header("Extra Params")]
        [Range(0, 6)]
        public int ExtraDistance = 2;

        [Space]
        [Header("Advanced Step Reward")]
        [Range(0.0001f, 0.1f)][Tooltip("Higher value => less rewards/step")]
        public float StepRewardFrequency = 0.005f;
        [Range(1, 10)]public int DivRewardValue = 10;
        [Range(0.1f, 1f), Tooltip("sharp the value")]
        public float Epsilon = 0.4f;

        [Header("Passive Path Rewards")]
        public bool UsePathRewardData = true;
        [Range(1f, 10f)]public float PathRewardTotalValue = 1f;
        [Range(1f, 10f)]public int SpawnPathRewardAmount = 1;


    }
}
