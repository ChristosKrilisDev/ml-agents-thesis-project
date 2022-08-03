using Dijstra.path;
using UnityEngine;

namespace ML_Agents.Finder.Scripts
{
    public class NodePassage : MonoBehaviour
    {

        [SerializeField]
        private Node currentNode;
        [SerializeField]
        private Node passageNode;
        //
        // public void OnPlayerPass()
        // {
        //     (currentNode, passageNode) = (passageNode, currentNode);
        // }

        public float NodePassageDistance()
        {
            return Vector2.Distance(currentNode.transform.position, passageNode.transform.position);
        }

        public Node GetCurrentNode => currentNode;
        public Node GetPassageNode => passageNode;

    }
}
