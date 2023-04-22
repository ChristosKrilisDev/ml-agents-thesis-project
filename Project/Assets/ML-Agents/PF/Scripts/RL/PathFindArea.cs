using System.Collections.Generic;
using JetBrains.Annotations;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.UtilsScripts;
using UnityEngine;

namespace ML_Agents.PF.Scripts.RL
{
    public class PathFindArea : MonoBehaviour
    {
        [SerializeField] private PathRewardManager _pathRewardManager;
        [SerializeField] private GameObject[] _spawnAreas;
        [SerializeField]private List<Wall> _walls;

        public GameObject[] SpawnAreas => _spawnAreas;

        private static GameObject GoalNodePref => Utils.FinalNode;
        private static GameObject BlockPref => Utils.SimpleNode;
        private static GameObject RewardNodePref => Utils.RewardNode;



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

        //todo :
        //revile rewards after hitting the check Point, or hide them if phase >= 3
        // move add reward to State machine from agent OnTriiger ender
        public PathFindArea CreateRewardNode(Transform nodePos, Transform prevNodePos)
        {
            _pathRewardManager.SpawnRewards(nodePos, prevNodePos);
            // var pathReward = Instantiate(RewardNodePref, Vector3.zero, Quaternion.Euler(0f, 0f, 0f),transform);
            // pathReward.transform.position = nodePos.position;
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


        public void RandomizeWalls()
        {

            if (GameManager.Instance.RewardData.HideWalls == false)
            {
                foreach (var wall in _walls) wall.Hide();
                return;
            }

            if (GameManager.Instance.RewardData.UseRandomWallWidth == false) return;
            foreach (var wall in _walls) wall.RandomizeSize();
        }


        public void CleanArea()
        {
            foreach (var area in _spawnAreas)
            {
                area.gameObject.transform.GetComponent<MeshRenderer>().enabled = false;
            }

            foreach (Transform child in transform)
            {
                if (child.CompareTag(TagData.OBJECT_TAG)) Destroy(child.gameObject);
                // if (child.CompareTag(TagData.PATH_REWARD_TAG)) Destroy(child.gameObject);
                _pathRewardManager.Reset();
            }
        }
    }
}
