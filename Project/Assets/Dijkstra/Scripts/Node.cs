using System.Collections.Generic;
using UnityEngine;

namespace Dijkstra.Scripts
{
    public sealed class Node : MonoBehaviour
    {
        [SerializeField] private List<Node> _connections = new List<Node>();

        public List<Node> Connections
        {
            get => _connections;
            set => _connections = value;
        }
    }
}
