using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dijstra.path
{
    public class GraphManager : MonoBehaviour
    {
        public List<Node> nodes = new List<Node>();
        public Node[,] nodes2D;
        //Dictionary<int, int> graphs = new Dictionary<int, int>();

        public void ConnectNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    
                    if(nodes2D[i, j] != nodes2D[i, j])
                    {
                        if (nodes2D.GetValue(j) == nodes2D.GetValue(j)) //right
                            nodes[i].connections.Add(nodes[i+1]); 
                        if (nodes2D[i, j] == nodes2D[i+1, j])   //above
                            nodes[i].connections.Add(nodes[j + 1]);

                        else break;
                    }
                }
            }

        }

        private void Start()
        {
            ConnectNodes();
        }
    }
}
