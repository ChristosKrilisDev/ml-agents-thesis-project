using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ML_Agents.Finder.Scripts
{
    public static class EpisodeHandler
    {
        

        /// <summary>
        /// if any condition is true then the episode is done and return true
        /// </summary>
        /// <param name="conditions">expected conditions : OnTargetFind, OnHitWall, OnReachMaxStep</param>
        /// <returns>on any true condition</returns>
        public static bool HasEpisodeEnded(IEnumerable<bool> conditions)
        {
            return conditions.Any(condition => condition);
        }

        /// <summary>
        /// Check the distance ~ Dijkstra
        /// </summary>
        /// <returns>true if the distance that agent did is less than dijstras shortest path length</returns>
        public static bool CompareCurrentDistance(float currentDistanceTraveled, float pathLength, float multiplier = 1)
        {
            return currentDistanceTraveled <= pathLength * multiplier;
        }

        /// <summary>
        /// used to find the distance between agent and current goal
        /// </summary>
        /// <param name="pointA">Agent</param>
        /// <param name="pointB">goal</param>
        /// <returns>the distance in float, from pointA to pointB</returns>
        public static float GetDistanceDifference(GameObject pointA, GameObject pointB)
        {
            if (!pointA || !pointB) return -1;
            //get local transforms
            var localPositionA = pointA.transform.localPosition;
            var localPositionB = pointB.transform.localPosition;

            var pA = new Vector3(localPositionA.x, 0, localPositionA.z);
            var pB = new Vector3(localPositionB.x, 0, localPositionB.z);

            return Vector3.Distance(pA, pB);
        }

        // private void SendData() //dont use
        // {
        //     if (_isFirstTake) return;
        //
        //     _isFirstTake = false;
        //     GameManager.instance.WriteData(_episodeCounter, _distanceRecorder.GetTraveledDistance, _pathTotalLength,
        //         _hasFindGoal, 0);
        // }

    }
}
