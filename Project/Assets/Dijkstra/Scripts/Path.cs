using System.Collections.Generic;
using System.Linq;


namespace Dijstra.path
{
    public sealed class Path
    {
        public List<Node> PathNodes => _pathNodes;
        private List<Node> _pathNodes = new List<Node>();

        public float Length => _length;
        private float _length = 0f;

        public Path()
        {
            _pathNodes.Clear();
            _length = 0;
        }

        public void Bake()
        {
            _length = 0f;
            var calculated = new List<Node>();

            foreach (var node in _pathNodes)
            {
                foreach (var connection in node.connections)
                {
                    if (_pathNodes.Contains(connection) && !calculated.Contains(connection))
                    {
                        //Bug : Length dosnt reset
                        _length += 1;
                    }
                }
                calculated.Add(node);
            }
        }

        public override string ToString()
        {
            return string.Format("Nodes: {0} Path Length: {1}",
                string.Join(", ", PathNodes.Select(node => node.name).ToArray()),
                Length);
        }

    }
}
