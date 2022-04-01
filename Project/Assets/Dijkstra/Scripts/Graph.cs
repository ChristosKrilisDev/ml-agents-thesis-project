using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The Graph.
/// </summary>
///
namespace Dijstra.path
{
    public class Graph : MonoBehaviour
    {
        [SerializeField]
        protected List<Node> m_Nodes = new List<Node>();

        [HideInInspector]
        public Node m_Start;
        [HideInInspector]
        public Node m_End;

        protected Path m_Path = new Path();

        public virtual List<Node> nodes
        {
            get { return m_Nodes; }
        }

        //test
        //private void Start()
        //{
        //    ConnectNodes();
        //}

        #region ConnectNodes
        /// <summary>
        /// auto connect nodes of a 1D list
        /// in a 2D grid, connect the nodes based on the "connectionRange" var
        /// </summary>
        ///
        [Tooltip("Step : the incresment the pointer should take to connect above/below nodes")]
        const int gridStep = 3;                   // for up/down connections
        const int connectionRange = 1;            // for left/right connections
        public void ConnectNodes()
        {
            int cp = 0; //check point, 2 == right edge, 0 == right edge

            for (int i = 0; i < m_Nodes.Count; i++)
            {
                if (cp >= gridStep)     //reset checkpoint when reache step value
                    cp = 0;

                //right
                if (i + connectionRange < m_Nodes.Count)
                    if (cp < gridStep - 1) m_Nodes[i].connections.Add(m_Nodes[i + connectionRange]);
                //left
                if (i - connectionRange >= 0)
                    if (cp != 0) m_Nodes[i].connections.Add(m_Nodes[i - connectionRange]);
                //up
                if (i + gridStep < m_Nodes.Count) m_Nodes[i].connections.Add(m_Nodes[i + gridStep]);
                //down
                if (i - gridStep >= 0) m_Nodes[i].connections.Add(m_Nodes[i - gridStep]);

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

            // We don't accept null arguments
            if (start == null || end == null)
            {
                throw new ArgumentNullException();
            }

            // The final path
            Path path = new Path();

            // If the start and end are same node, we can return the start node
            if (start == end)
            {
                path.nodes.Add(start);
                return path;
            }

            // The list of unvisited nodes
            List<Node> unvisited = new List<Node>();

            // Previous nodes in optimal path from source
            Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

            // The calculated distances, set all to Infinity at start, except the start Node
            Dictionary<Node, float> distances = new Dictionary<Node, float>();

            for (int i = 0; i < m_Nodes.Count; i++)
            {
                Node node = m_Nodes[i];
                unvisited.Add(node);

                // Setting the node distance to Infinity
                distances.Add(node, float.MaxValue);
            }

            // Set the starting Node distance to zero
            distances[start] = 0f;
            while (unvisited.Count != 0)
            {

                // Ordering the unvisited list by distance, smallest distance at start and largest at end
                unvisited = unvisited.OrderBy(node => distances[node]).ToList();

                // Getting the Node with smallest distance
                Node current = unvisited[0];

                // Remove the current node from unvisisted list
                unvisited.Remove(current);

                // When the current node is equal to the end node, then we can break and return the path
                if (current == end)
                {

                    // Construct the shortest path
                    while (previous.ContainsKey(current))
                    {

                        // Insert the node onto the final result
                        path.nodes.Insert(0, current);

                        // Traverse from start to end
                        current = previous[current];
                    }

                    // Insert the source onto the final result
                    path.nodes.Insert(0, current);
                    break;
                }

                // Looping through the Node connections (neighbors) and where the connection (neighbor) is available at unvisited list
                for (int i = 0; i < current.connections.Count; i++)
                {
                    Node neighbor = current.connections[i];

                    // Getting the distance between the current node and the connection (neighbor)
                    float length = Vector3.Distance(current.transform.position, neighbor.transform.position);

                    // The distance from start node to this connection (neighbor) of current node
                    float alt = distances[current] + length;

                    // A shorter path to the connection (neighbor) has been found
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }
            path.Bake();
            return path;
        }

        #endregion

    }
}
