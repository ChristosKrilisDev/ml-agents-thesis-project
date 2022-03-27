using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dijstra.path
{
    public class GraphManager : MonoBehaviour
    {
        [Tooltip("Step : the incresment the pointer should take to connect above/below nodes")]
        private int step = 3;



        //private int rangeOfConnections = 1; // connect nodes 1 step distance

        public List<Node> nodes = new List<Node>();

        //2D array

        // step = +1

        // search for node[i,j]
        // where availabe node on
        // [i+1,j] , [i-1,j] , [i, j +1] and [i, j-1]
        //limits :
        // i|j -1 < 0 break;
        // i|j +1 > list.count break;

        public void ConnectNodes()
        {
            int cp = 0; //check point, 2 == right edge, 0 == right edge

            for (int i = 0; i < nodes.Count; i++)
            {
                if (cp >= step)     //reset checkpoint when reache step value
                    cp = 0;

                //right
                if (i + 1 < nodes.Count)
                    if (cp < step - 1) nodes[i].connections.Add(nodes[i + 1]);
                //left
                if (i - 1 >= 0)
                    if (cp != 0) nodes[i].connections.Add(nodes[i - 1]);
                //up
                if (i + step < nodes.Count) nodes[i].connections.Add(nodes[i + step]);
                //down
                if (i - step >= 0) nodes[i].connections.Add(nodes[i - step]);

                cp++;
            }

        }

        private void Start()
        {
            ConnectNodes();
        }

    }
}
