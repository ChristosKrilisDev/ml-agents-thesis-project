using UnityEngine;

namespace ML_Agents.Finder.Scripts
{
    [CreateAssetMenu(fileName = "RewardData", menuName = "RL/RewardData", order = 0)]
    public class RewardData : ScriptableObject
    {

        [Range(0, -1)]
        public float WallPenalty = 10;

        public float FoundGoalReward = 10;

        public float StepReward = 1;

        public float FailedEpisode = 10;

        public float SuccessEpisode = 10;

        public int ExtraDistance = 2;
    }
}
