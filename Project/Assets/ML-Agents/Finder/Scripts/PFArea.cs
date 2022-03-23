using UnityEngine;
using Unity.MLAgentsExamples;

public class PFArea : Area
{
    public GameObject goalPref;
    public GameObject blockPref;
    public GameObject[] spawnAreas;
    public int numPyra;
    public float range;

    public void CreateGoalObject(int numObjects, int spawnAreaIndex)
    {
        CreateObject(numObjects, goalPref, spawnAreaIndex);
    }

    public void CreateBlockObject(int numObjects, int spawnAreaIndex)
    {
        CreateObject(numObjects, blockPref, spawnAreaIndex);
    }

    void CreateObject(int numObjects, GameObject desiredObject, int spawnAreaIndex)
    {
        for (var i = 0; i < numObjects; i++)
        {
            var newObject = Instantiate(desiredObject, Vector3.zero,Quaternion.Euler(0f, 0f, 0f), transform);
            PlaceObject(newObject, spawnAreaIndex);
        }
    }

    public void PlaceObject(GameObject objectToPlace, int spawnAreaIndex)
    {
        var spawnTransform = spawnAreas[spawnAreaIndex].transform;
        var xRange = spawnTransform.localScale.x / 2.1f;
        var zRange = spawnTransform.localScale.z / 2.1f;

        objectToPlace.transform.position = new Vector3(Random.Range(-xRange, xRange), 2f, Random.Range(-zRange, zRange))
            + spawnTransform.position;
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
