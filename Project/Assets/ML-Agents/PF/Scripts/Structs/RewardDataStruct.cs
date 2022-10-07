namespace ML_Agents.PF.Scripts.Structs
{
    /// <summary>
    /// Data needed to calculate the complex reward fuction
    /// </summary>
    public struct RewardDataStruct
    {
        public bool HasEpisodeEnd;
        public bool[] Conditions;
        public float CurrentDistance;
        public float CurrentTargetDistance;
    }
}
