using UnityEngine;


namespace ML_Agents.Finder.Scripts
{
    public class PfArea : MonoBehaviour
    {
        [SerializeField]
        private GameObject _goalNodePref;
        [SerializeField]
        private GameObject _blockPref;
        [SerializeField]
        private GameObject[] _spawnAreas;

        private Transform[] _nodes;

        public GameObject GoalNode { get; private set; }

        public void SetNodesPosition(ref Transform[] nodes)
        {
            _nodes = nodes;
        }


        public GameObject CreateGoalNode(int spawnAreaIndex)
        {
            GoalNode = CreateNode(_goalNodePref, spawnAreaIndex);
            return GoalNode;
        }

        public PfArea CreateBlockNode(int spawnAreaIndex)
        {
            CreateNode(_blockPref, spawnAreaIndex);
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
            {
                var spawnTransform = _spawnAreas[spawnAreaIndex].transform;
                var localScale = spawnTransform.localScale;
                var xRange = localScale.x / 2.1f;
                var zRange = localScale.z / 2.1f;

                objectToPlace.transform.position =
                    new Vector3(Random.Range(-xRange, xRange), 2f, Random.Range(-zRange, zRange)) + spawnTransform.position;
            }
            SetNodePositionInternal(spawnAreaIndex, objectToPlace);
        }


        private void SetNodePositionInternal(int spawnIndex, GameObject node)
        {
            _nodes[spawnIndex].position = node.transform.position;
        }

        public void CleanArea()
        {
            foreach (Transform child in transform)
                if (child.CompareTag("pfobj"))
                    DestroyImmediate(child.gameObject);
        }

        public PfArea HideCallBack(int nodeIndex)
        {
            //_nodes[nodeIndex].gameObject.SetActive(false);
            return this;
        }
    }
}
