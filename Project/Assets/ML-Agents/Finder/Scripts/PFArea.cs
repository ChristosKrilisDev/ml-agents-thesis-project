using UnityEngine;
using Unity.MLAgentsExamples;
using System.Collections.Generic;

public class PFArea : Area
{
    Transform[] nodes;

    public GameObject goalPref;
    public GameObject blockPref;
    public GameObject[] spawnAreas;
    public int numPyra;
    public float range;

    public GameObject CreateGoalObject(int numObjects, int spawnAreaIndex )
    {
        GameObject goal = CreateObject(numObjects, goalPref, spawnAreaIndex);
        return goal;
    }

    public void CreateBlockObject(int numObjects, int spawnAreaIndex )
    {
        CreateObject(numObjects, blockPref, spawnAreaIndex);
    }

    GameObject CreateObject(int numObjects, GameObject desiredObject, int spawnAreaIndex )
    {
        GameObject goal = null; // get the goal object and use it on CheckPoint.cs
        for (var i = 0; i < numObjects; i++)
        {
            var newObject = Instantiate(desiredObject, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), transform);
            goal = newObject as GameObject;
            PlaceObject(newObject, spawnAreaIndex);

        }
        return goal;
    }

    public void PlaceObject(GameObject objectToPlace, int spawnAreaIndex)
    {
        var spawnTransform = spawnAreas[spawnAreaIndex].transform;
        var xRange = spawnTransform.localScale.x / 2.1f;
        var zRange = spawnTransform.localScale.z / 2.1f;

        objectToPlace.transform.position = new Vector3(Random.Range(-xRange, xRange), 2f, Random.Range(-zRange, zRange))
            + spawnTransform.position;


        SetNodePos(spawnAreaIndex, objectToPlace);
    }



    void SetNodePos(int spawnIndex ,GameObject obj)
    {
        nodes[spawnIndex].transform.position = obj.transform.position;
    }


    public int SetNodesPosition(Transform[] nodes)
    {
        this.nodes = nodes;
        return 0;
        //for (int i = 0; i < nodes.Length; i++)
        //{
        //    this.nodes[i] = nodes[i];
        //}
    }

    public Transform[] GetNodesNewPosition()
    {
        return nodes;
    }

    public void CleanArea()
    {
        foreach (Transform child in transform)
            if (child.CompareTag("pfobj"))
            {
                Destroy(child.gameObject);
            }
    }

    public override void ResetArea()
    {
    }
}
