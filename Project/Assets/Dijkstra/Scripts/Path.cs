using System.Collections.Generic;
using System.Linq;

namespace Dijkstra.Scripts
{
    public sealed class Path
    {
        public List<Node> PathNodes => _pathNodes;
        private readonly List<Node> _pathNodes = new List<Node>();

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

            _length = 0f;

            foreach (var node in _pathNodes)
            {
                flagCounter++;

                foreach (var connection in node.Connections)
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
        }

        public override string ToString()
        {
            return $"{string.Join(",", PathNodes.Select(node => node.name).ToArray())},Path Length: {Length}";
        }

    }
}
