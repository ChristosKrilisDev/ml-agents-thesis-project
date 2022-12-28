using System.Collections.Generic;
using Dijkstra.Scripts;
using UnityEngine;
namespace ML_Agents.PF.Scripts.RL
{
    public class NodeMapping
    {
        private readonly Dictionary<GameObject, bool> _nodesDictionary = new Dictionary<GameObject, bool>();
        private readonly GameObject[] _nodes;

        public NodeMapping(GameObject[] nodes)
        {
            _nodes = nodes;

            foreach (var node in _nodes)
            {
                _nodesDictionary.Add(node, false);
            }
        }

        public bool CheckMap(GameObject node)
        {
            //agent has already visited this node
            if (_nodesDictionary.ContainsKey(node) && _nodesDictionary[node] == true)
            {
                return true;
            }

            AddVisitedNode(node);
            return false;
        }

        private void AddVisitedNode(GameObject node)
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
