using System.Collections.Generic;
using Dijkstra.Scripts;
namespace ML_Agents.PF.Scripts.RL
{
    public class NodeMapping
    {
        private readonly Dictionary<Node, bool> _nodesDictionary = new Dictionary<Node, bool>();
        private readonly List<Node> _nodes;

        public NodeMapping(List<Node> nodes)
        {
            _nodes = nodes;

            foreach (var node in _nodes)
            {
                _nodesDictionary.Add(node, false);
            }
        }

        public bool CheckMap(Node node)
        {
            //agent has already visited this node
            if (_nodesDictionary.ContainsKey(node)) return true;

            AddVisitedNode(node);
            return false;
        }

        private void AddVisitedNode(Node node)
        {
            _nodesDictionary[node] = true;
        }


        public void Reset()
        {
            foreach (var node in _nodes)
            {
                _nodesDictionary[node] = false;
            }
        }

    }
}
