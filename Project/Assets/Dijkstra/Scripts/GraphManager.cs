using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dijstra.path
{
    public class GraphManager : MonoBehaviour
    {
        [Tooltip("Step : the incresment the pointer should take to connect above/below nodes")]
        const int gridStep = 3;                   // for up/down connections
        const int connectionRange = 1;     // for left/right connections

        public List<Node> nodes = new List<Node>();


        private void Start()
        {
            ConnectNodes();
        }



        public void ConnectNodes()
        {
            int cp = 0; //check point, 2 == right edge, 0 == right edge

            for (int i = 0; i < nodes.Count; i++)
            {
                if (cp >= gridStep)     //reset checkpoint when reache step value
                    cp = 0;

                //right
                if (i + connectionRange < nodes.Count)
                    if (cp < gridStep - 1) nodes[i].connections.Add(nodes[i + connectionRange]);
                //left
                if (i - connectionRange >= 0)
                    if (cp != 0) nodes[i].connections.Add(nodes[i - connectionRange]);
                //up
                if (i + gridStep < nodes.Count) nodes[i].connections.Add(nodes[i + gridStep]);
                //down
                if (i - gridStep >= 0) nodes[i].connections.Add(nodes[i - gridStep]);

                cp++;
            }

        }

    }
}
