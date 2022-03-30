using UnityEngine;
using Unity.MLAgentsExamples;
using System.Collections.Generic;

public class PFArea : Area
{
    public List<Transform> spawnedObjects;


    public GameObject goalPref;
    public GameObject blockPref;
    public GameObject[] spawnAreas;
    public int numPyra;
    public float range;

    public void CreateGoalObject(int numObjects, int spawnAreaIndex, Transform node)
    {
        CreateObject(numObjects, goalPref, spawnAreaIndex, node);
    }

    public void CreateBlockObject(int numObjects, int spawnAreaIndex, Transform node)
    {
        CreateObject(numObjects, blockPref, spawnAreaIndex, node);
    }

    void CreateObject(int numObjects, GameObject desiredObject, int spawnAreaIndex, Transform node)
    {
        for (var i = 0; i < numObjects; i++)
        {
            var newObject = Instantiate(desiredObject, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), transform);
            PlaceObject(newObject, spawnAreaIndex, node);
        }
    }

    public void PlaceObject(GameObject objectToPlace, int spawnAreaIndex, Transform node)
    {
        var spawnTransform = spawnAreas[spawnAreaIndex].transform;
        var xRange = spawnTransform.localScale.x / 2.1f;
        var zRange = spawnTransform.localScale.z / 2.1f;

        objectToPlace.transform.position = new Vector3(Random.Range(-xRange, xRange), 2f, Random.Range(-zRange, zRange))
            + spawnTransform.position;

        spawnedObjects.Add(objectToPlace.transform);

        //move node to the same location as the object
        //node.position = objectToPlace.transform.position;
    }

    void CreateArea(Transform[] nodes)
    {
        CreateBlockObject(1, 0, nodes[3].transform);
        CreateBlockObject(1, 1, nodes[3].transform);
        CreateBlockObject(1, 2, nodes[3].transform);
        CreateBlockObject(1, 3, nodes[3].transform);
        CreateBlockObject(1, 4, nodes[3].transform);
        CreateBlockObject(1, 5, nodes[3].transform);



    }


    public Transform[] GetNodesPosition(Transform[] nodes)
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            nodes[i].position = spawnedObjects[i].position;
        }
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
