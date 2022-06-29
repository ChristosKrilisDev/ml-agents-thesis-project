using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dijstra.path
{

    public sealed class Path
    {

        private List<Node> m_Nodes = new List<Node>();
        private float m_Length = 0f;


        public List<Node> nodes => m_Nodes;
        public List<float> pDistances = new List<float>();
        public float length =>  m_Length; 
        

        /// <summary>
        /// Bake the path.
        /// Making the path ready for usage, Such as calculating the length.
        /// </summary>
        public void Bake()
        {
            pDistances.Clear();
            var calculated = new List<Node>();
            m_Length = 0f;
            for (var i = 0; i < m_Nodes.Count; i++)
            {
                var node = m_Nodes[i];
                for (var j = 0; j < node.connections.Count; j++)
                {
                    var connection = node.connections[j];

                    // Don't calculate calculated nodes
                    if (m_Nodes.Contains(connection) && !calculated.Contains(connection))
                    {

                        // Calculating the distance between a node and connection when they are both available in path nodes list
                        //m_Length += Vector3.Distance(node.transform.position, connection.transform.position);
                        m_Length += 1;
                       // Debug.Log("Path Length => " + m_Length);
                    }
                }
                calculated.Add(node);
                pDistances.Add(m_Length);
            }
            //Debug.Log( ToString());
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            //\nLength: {1}
            return string.Format("Nodes: {0} Path Length: {1}", 
                string.Join(", ", nodes.Select(node => node.name).ToArray()),
                length);
        }

    }
}
