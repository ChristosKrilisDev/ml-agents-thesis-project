using System;
using UnityEngine;
namespace ML_Agents.PF.Scripts.Data
{
    public class ConditionsData
    {

        /// <summary>
        /// Contains the data condition related data
        /// </summary>

        public int MaxStep;
        public int StepCount;
        public float StepFactor;

        public int TraveledDistance;
        public int CheckPointLength;
        public int PathTotalLength;

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
            MaxStep= 0;
            StepCount= 0;
            StepFactor= 0;
            TraveledDistance= -1;
            CheckPointLength= 0;
            PathTotalLength= 0;

            HasTouchedWall= false;
            HasFoundGoal= false;
            HasFoundCheckpoint= false;
        }

    }
}
