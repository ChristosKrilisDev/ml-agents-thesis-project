using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Dijstra.path
{
    public class Graph : MonoBehaviour
    {
        public List<Node> Nodes => m_Nodes;
        [SerializeField] protected List<Node> m_Nodes = new List<Node>();

        [HideInInspector] public Node StartNode;
        [HideInInspector] public Node CheckPointNode;
        [HideInInspector] public Node EndNode;

        
        public bool VisualizePath = false;

        private void Awake()
        {
            m_Nodes.Clear();

            var childrenNodes = transform.GetComponentsInChildren<Node>();
            foreach (var node in childrenNodes) m_Nodes.Add(node);
            
        }

    #region ConnectNodes

        /// <summary>
        /// auto connect nodes of a 1D list
        /// in a 2D grid, connect the nodes based on the "connectionRange" var
        /// </summary>
        
        [Tooltip("Step : the incresment the pointer should take to connect above/below nodes")]
        private const int GRID_STEP = 3; // for up/down connections
        private const int CONNECTION_RANGE = 1; // for left/right connections
        public void ConnectNodes()
        {
            var cp = 0; //check point, 2 == right edge, 0 == right edge

            for (var i = 0; i < m_Nodes.Count; i++)
            {
                if (cp >= GRID_STEP) //reset checkpoint when reach step value
                    cp = 0;

                //right
                if (i + CONNECTION_RANGE < m_Nodes.Count)
                    if (cp < GRID_STEP - 1)
                        m_Nodes[i].connections.Add(m_Nodes[i + CONNECTION_RANGE]);
                //left
                if (i - CONNECTION_RANGE >= 0)
                    if (cp != 0)
                        m_Nodes[i].connections.Add(m_Nodes[i - CONNECTION_RANGE]);
                //up
                if (i + GRID_STEP < m_Nodes.Count) m_Nodes[i].connections.Add(m_Nodes[i + GRID_STEP]);
                //down
                if (i - GRID_STEP >= 0) m_Nodes[i].connections.Add(m_Nodes[i - GRID_STEP]);

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
        public virtual Path GetShortestPath(Node start, Node end)
        {
            VisualizePath = false;
            
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

            for (var i = 0; i < m_Nodes.Count; i++)
            {
                var node = m_Nodes[i];
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
                
                for (var i = 0; i < current.connections.Count; i++)
                {
                    var neighbor = current.connections[i];
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
            VisualizePath = true;
            return path;
        }


    #endregion

    }
}
