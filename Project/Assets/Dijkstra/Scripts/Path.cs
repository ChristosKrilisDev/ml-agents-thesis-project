using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dijstra.path
{
    public sealed class Path
    {
        private List<Node> _pathNodes = new List<Node>();
        private float _length = 0f;


        public List<Node> PathNodes => _pathNodes;
        public List<float> pDistances = new List<float>();
        public float length =>  _length; 
        
        
        public void Bake()
        {
            pDistances.Clear();
            _length = 0f;

            var calculated = new List<Node>();

            foreach (var node in _pathNodes)
            {
                foreach (var connection in node.connections)
                {
                    if (_pathNodes.Contains(connection) && !calculated.Contains(connection))
                    {
                        // Calculating the distance between a node and connection when they are both available in path nodes list
                        _length += 1;
                    }
                }

                calculated.Add(node);
                pDistances.Add(_length);
            }
            Debug.Log(ToString());
        }


        public override string ToString()
        {
            return string.Format("Nodes: {0} Path Length: {1}", 
                string.Join(", ", PathNodes.Select(node => node.name).ToArray()),
                length);
        }

    }
}
