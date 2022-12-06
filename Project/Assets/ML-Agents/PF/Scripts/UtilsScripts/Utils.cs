using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEditor;
using UnityEngine;
namespace ML_Agents.PF.Scripts.UtilsScripts
{
    public static class Utils
    {

        public static void GatherAssets()
        {
            GetButtonMaterials();
            GetPrefabsNodesMaterials();
        }

    #region GetButtonMats

        private const string MATERIALS_ROOT_PATH = "Assets/ML-Agents/SharedAssets/Materials";
        public static Material OnButtonMaterial;
        public static Material OffButtonMaterial;

        private static void GetButtonMaterials()
        {
#if UNITY_EDITOR
            OnButtonMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_ROOT_PATH}/Green.mat");
            OffButtonMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_ROOT_PATH}/Orange.mat");
#endif
        }

    #endregion

    #region GetPrefabs

        private const string PREFABS_ROOT_PATH = "Assets/ML-Agents/PF/Prefabs";
        public static GameObject FinalNode;
        public static GameObject SimpleNode;

        private static void GetPrefabsNodesMaterials()
        {
#if UNITY_EDITOR
            FinalNode = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_ROOT_PATH}/FinalGoal.prefab");
            SimpleNode = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_ROOT_PATH}/BlockObj.prefab");
#endif
        }

    #endregion

    #region In-Game Useful methods

        /// <summary>
        /// if any condition is true then the episode is done and return true
        /// </summary>
        /// <param name="conditions">expected conditions : OnTargetFind, OnHitWall, OnReachMaxStep</param>
        /// <returns>on any true condition</returns>
        public static bool HasEpisodeEnded(IEnumerable<bool> conditions)
        {
            foreach (var condition in conditions)
            {
                if (condition) return true;
            }

            return false;
        }

        /// <summary>
        /// Check the distance ~ Dijkstra
        /// </summary>
        /// <returns>true if the distance that agent did is less than dijstras shortest path length</returns>
        public static bool IsCurrDistLessThanPathLength(float currentDistanceTraveled, float pathLength, bool useExtraReward = true)
        {
            if (useExtraReward)
            {
                return currentDistanceTraveled <= pathLength + GameManager.Instance.RewardData.ExtraDistance;
            }

            return currentDistanceTraveled <= pathLength;
        }

        /// <summary>
        /// used to find the distance between agent and current goal
        /// </summary>
        /// <param name="startPoint">Agent</param>
        /// <param name="endPoint">goal</param>
        /// <returns>the distance in float, from pointA to pointB</returns>
        public static float GetDistanceDifference(GameObject startPoint, GameObject endPoint)
        {
            if (!startPoint || !endPoint) return -1;
            //get local transforms
            var localPositionA = startPoint.transform.localPosition;
            var localPositionB = endPoint.transform.localPosition;

            var pA = new Vector3(localPositionA.x, 0, localPositionA.z);
            var pB = new Vector3(localPositionB.x, 0, localPositionB.z);

            return Vector3.Distance(pA, pB);
        }

        public static bool NearlyEqual(float a, float b, float epsilon)
        {
            const float minNormal = 1.17549435f;
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

            if (a.Equals(b))
            {
                // shortcut, handles infinities
                return true;
            }

            if (a == 0 || b == 0 || absA + absB < minNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * minNormal);
            }

            // use relative error
            return diff / (absA + absB) < epsilon;

        }

    #endregion

    #region TENSOR BOARD DATA

        public static void WriteDijkstraData(string key, bool result)
        {
            var followedDijkstra = result ? 1 : 0;
            Academy.Instance.StatsRecorder.Add(key, followedDijkstra, StatAggregationMethod.Average);
        }

    #endregion

    }
}
