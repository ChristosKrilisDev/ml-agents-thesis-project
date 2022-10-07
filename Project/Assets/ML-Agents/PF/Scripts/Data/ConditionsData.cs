using UnityEngine;

namespace ML_Agents.PF.Scripts.Data
{
    public class ConditionsData
    {

        /// <summary>
        /// Contains the data condition related data
        /// </summary>

        public int MaxStep = 1500;
        public int StepCount;
        public float StepFactor;

        public int TraveledDistance;
        public int CheckPointPathLength;
        public int FullPathLength;

        public bool HasTouchedWall;
        public bool HasFoundGoal;
        public bool HasFoundCheckpoint;

        public ConditionsData()
        {
            Debug.Log("New Condition data created");
            Reset();
        }

        public void Reset()
        {
            MaxStep= 1500;
            StepCount= 0;
            StepFactor= 0;
            TraveledDistance= -1;
            CheckPointPathLength= 0;
            FullPathLength= 0;

            HasTouchedWall= false;
            HasFoundGoal= false;
            HasFoundCheckpoint= false;
        }

    }
}
