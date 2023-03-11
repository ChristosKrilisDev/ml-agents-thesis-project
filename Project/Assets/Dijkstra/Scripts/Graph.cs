using System.Collections.Generic;
using UnityEngine;

namespace Dijkstra.Scripts
{
    public sealed class Graph : MonoBehaviour
    {
        private readonly List<Node> _nodes = new List<Node>();

        public List<Node> Nodes => _nodes;
        [HideInInspector] public Node StartNode;
        [HideInInspector] public Node CheckPointNode;
        [HideInInspector] public Node EndNode;
        [HideInInspector] public bool IsAbleToVisualizePath = true;

        private void Awake()
        {
            _nodes.Clear();

            var childrenNodes = transform.GetComponentsInChildren<Node>();
            foreach (var node in childrenNodes) _nodes.Add(node);
        }

    #region ConnectNodes

        /// <summary>
        /// auto connect nodes of a 1D list
        /// in a 2D grid, connect the nodes based on the "connectionRange" var
        /// </summary>
        [Tooltip("Step : the increment the pointer should take to connect above/below nodes")]
        private const int GRID_STEP = 3; // for up/down connections
        private const int CONNECTION_RANGE = 1; // for left/right connections

        public void ConnectNodes()
        {
            var cp = 0; //check point, 2 == right edge, 0 == left edge

            for (var i = 0; i < _nodes.Count; i++)
            {
                //reset checkpoint when reach step value
                if (cp >= GRID_STEP) cp = 0;

                if (i + CONNECTION_RANGE < _nodes.Count)
                {
                    //you have a right connection
                    if (cp < GRID_STEP - 1) _nodes[i].Connections.Add(_nodes[i + CONNECTION_RANGE]);
                }

                if (i - CONNECTION_RANGE >= 0)
                {
                    //you have a left connection
                    if (cp != 0) _nodes[i].Connections.Add(_nodes[i - CONNECTION_RANGE]);
                }

                //up
                if (i + GRID_STEP < _nodes.Count) _nodes[i].Connections.Add(_nodes[i + GRID_STEP]);
                //down
                if (i - GRID_STEP >= 0) _nodes[i].Connections.Add(_nodes[i - GRID_STEP]);

                cp++;
            }
        }

    #endregion

        public Path GetShortestPath(Node start, Node end)
        {
            IsAbleToVisualizePath = true;
            return Dijkstra.GetShortestPath(start, end, _nodes);
        }

    }
}
