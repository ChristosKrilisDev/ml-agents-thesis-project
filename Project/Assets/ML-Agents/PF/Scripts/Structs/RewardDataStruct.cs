namespace ML_Agents.PF.Scripts.Structs
{
    public struct RewardDataStruct
    {
        public bool HasEpisodeEnd;
        public bool[] Conditions;
        public float CurrentDistance;
        public float CurrentTargetDistance;
    }
}
