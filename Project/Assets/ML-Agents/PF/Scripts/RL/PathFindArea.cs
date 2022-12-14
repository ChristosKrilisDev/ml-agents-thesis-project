using System.Collections.Generic;
using ML_Agents.PF.Scripts.UtilsScripts;
using UnityEngine;

namespace ML_Agents.PF.Scripts.RL
{
    public class PathFindArea : MonoBehaviour
    {
        [SerializeField] private GameObject[] _spawnAreas;
        public GameObject[] SpawnAreas => _spawnAreas;

        private static GameObject GoalNodePref => Utils.FinalNode;
        private static GameObject BlockPref => Utils.SimpleNode;
        private static GameObject RewardNodePref => Utils.RewardNode;

        private const string OBJECT_TAG = "pfobj";

        private Transform[] _nodes;
        public Transform[] Nodes => _nodes;

        private GameObject GoalNode { get; set; }

        public void SetNodesPosition(ref Transform[] nodes)
        {
            _nodes = nodes;
        }

        public GameObject CreateGoalNode(int spawnAreaIndex)
        {
            GoalNode = CreateNode(GoalNodePref, spawnAreaIndex);

            return GoalNode;
        }

        public PathFindArea CreateBlockNode(int spawnAreaIndex)
        {
            CreateNode(BlockPref, spawnAreaIndex);

            return this;
        }

        public PathFindArea CreateRewardNode(Transform pos)
        {
            // CreateNode(RewardNodePref, spawnIndex);
            Instantiate(RewardNodePref, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), pos);
            return this;
        }

        private GameObject CreateNode(GameObject desiredObject, int spawnAreaIndex)
        {
            var newObject = Instantiate(desiredObject, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), transform);
            PlaceNode(newObject, spawnAreaIndex);

            return newObject;
        }

        public void PlaceNode(GameObject objectToPlace, int spawnAreaIndex)
        {

            var spawnTransform = _spawnAreas[spawnAreaIndex].transform;
            var localScale = spawnTransform.localScale;
            var xRange = localScale.x / 2.5f;
            var zRange = localScale.z / 2.5f;

            objectToPlace.transform.position =
                new Vector3(Random.Range(-xRange, xRange), 2f, Random.Range(-zRange, zRange)) + spawnTransform.position;

            SetNodePositionInternal(spawnAreaIndex, objectToPlace);
        }

        private void SetNodePositionInternal(int spawnIndex, GameObject node)
        {
            _nodes[spawnIndex].position = node.transform.position;
        }

        public void CleanArea()
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag(OBJECT_TAG)) Destroy(child.gameObject);
                if (child.CompareTag("reward")) Destroy(child.gameObject);
            }
        }
    }
}
