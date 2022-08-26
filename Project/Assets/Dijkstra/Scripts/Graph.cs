using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dijkstra.Scripts
{
    public sealed class Graph : MonoBehaviour
    {
        public List<Node> Nodes => _nodes;
        [SerializeField] private List<Node> _nodes = new List<Node>();

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
            var cp = 0; //check point, 2 == right edge, 0 == right edge

            for (var i = 0; i < _nodes.Count; i++)
            {
                //reset checkpoint when reach step value
                if (cp >= GRID_STEP) cp = 0;

                //right
                if (i + CONNECTION_RANGE < _nodes.Count)
                {
                    if (cp < GRID_STEP - 1) _nodes[i].Connections.Add(_nodes[i + CONNECTION_RANGE]);
                }
 
                //left
                if (i - CONNECTION_RANGE >= 0)
                {
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

    #region Dijktra

        /// <summary>
        /// DIJKSTRA
        /// Gets the shortest path from the starting Node to the ending Node.
        /// </summary>
        /// <returns>The shortest path.</returns>
        /// <param name="start">Start Node.</param>
        /// <param name="end">End Node.</param>
        public Path GetShortestPath(Node start, Node end)
        {
            if (start == null || end == null)
            {
                throw new ArgumentNullException();
            }

            var path = new Path();

            if (start == end)
            {
                path.PathNodes.Add(start);

                return path;
            }

            var unvisited = new List<Node>();
            var previous = new Dictionary<Node, Node>();
            var distances = new Dictionary<Node, float>();

            for (var i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                unvisited.Add(node);
                distances.Add(node, float.MaxValue);
            }

            distances[start] = 0f;

            while (unvisited.Count != 0)
            {
                // Ordering the unvisited list by distance, smallest distance at start and largest at end
                unvisited = unvisited.OrderBy(node => distances[node]).ToList();

                // Getting the Node with smallest distance
                var current = unvisited[0];
                unvisited.Remove(current);

                // When the current node is equal to the end node, then we can break and return the path
                if (current == end)
                {
                    // Construct the shortest path
                    while (previous.ContainsKey(current))
                    {
                        path.PathNodes.Insert(0, current);
                        current = previous[current];
                    }

                    path.PathNodes.Insert(0, current);

                    break;
                }

                for (var i = 0; i < current.Connections.Count; i++)
                {
                    var neighbor = current.Connections[i];
                    var length = Vector3.Distance(current.transform.position, neighbor.transform.position);
                    var alt = distances[current] + length;

                    // A shorter path to the connection (neighbor) has been found
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }
            path.Bake();
            IsAbleToVisualizePath = true;

            return path;
        }

    #endregion

    }
}
