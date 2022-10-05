namespace ML_Agents.PF.Scripts.Structs
{
    public struct RewardDataStruct
    {
        public bool HasEpisodeEnd;
        public bool[] Conditions;
        public float CurrentDistance;
        public float CurrentTargetDistance;

        // public RewardDataWrapper(bool hasEpisodeEnd, bool[] conditions, float currentDistance, float currentTargetDistance)
        // {
        //     HasEpisodeEnd = hasEpisodeEnd;
        //     Conditions = conditions;
        //     CurrentDistance = currentDistance;
        //     CurrentTargetDistance = currentTargetDistance;
        // }

    }
}
