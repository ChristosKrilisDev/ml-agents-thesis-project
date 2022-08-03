using System.Collections.Generic;
using UnityEngine;

namespace Dijstra.path
{
    public class Node : MonoBehaviour
    {
        [SerializeField]
        protected List<Node> m_Connections = new List<Node>();
        public Node this[int index] => m_Connections[index];

        public virtual List<Node> connections
        {
            get => m_Connections;
            set => m_Connections = value;
        }
    }
}
