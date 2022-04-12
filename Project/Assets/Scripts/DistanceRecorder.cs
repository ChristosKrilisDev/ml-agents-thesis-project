using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceRecorder : MonoBehaviour
{
    public float totalDistance = 0;
    [SerializeField]
    private bool record = true;
    private Vector3 previousLoc;
    void FixedUpdate()
    {
        if (record)
            RecordDistance();
    }
    void RecordDistance()
    {
        totalDistance += Vector3.Distance(transform.position, previousLoc);
        previousLoc = transform.position;
    }
    public void ToggleRecord() => record = !record;

}
