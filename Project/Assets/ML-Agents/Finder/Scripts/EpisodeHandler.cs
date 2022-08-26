using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ML_Agents.Finder.Scripts
{
    public static class EpisodeHandler
    {

        public static void Init()
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
            OnButtonMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_ROOT_PATH}/Green.mat");
            OffButtonMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_ROOT_PATH}/Orange.mat");
        }

    #endregion

    #region GetPrefabs

        private const string PREFABS_ROOT_PATH = "Assets/ML-Agents/Finder/Prefabs";
        public static GameObject FinalNode;
        public static GameObject SimpleNode;

        private static void GetPrefabsNodesMaterials()
        {
            FinalNode = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_ROOT_PATH}/FinalGoal.prefab");
            SimpleNode = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_ROOT_PATH}/BlockObj.prefab");
        }

    #endregion

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
    }
}
