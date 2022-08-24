using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dijstra.path
{
    public sealed class Path
    {
        public List<Node> PathNodes => _pathNodes;
        private List<Node> _pathNodes = new List<Node>();

        public float Length => _length;
        private float _length = 1f;

        public Path()
        {
            _pathNodes.Clear();
            _length = 1;
        }

        public void Bake()
        {
            var calculated = new List<Node>();
            var flagCounter = 0;
            
            _length = 1f;

            foreach (var node in _pathNodes)
            {
                flagCounter++;
                foreach (var connection in node.connections)
                {
                    //last node dosnt need to be calculated, add in the path and break
                    if (flagCounter == _pathNodes.Count)
                    {
                        calculated.Add(node);
                        break;
                    }

                    if (!_pathNodes.Contains(connection) && calculated.Contains(connection)) break;

                    _length += 1;
                    calculated.Add(node);
                    break;
                }
            }
            Debug.Log(ToString());
        }

        public override string ToString()
        {
            return $"Nodes: {string.Join(", ", PathNodes.Select(node => node.name).ToArray())} Path Length: {Length}";
        }

    }
}
