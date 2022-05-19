using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceRecorder : MonoBehaviour
{
    [SerializeField]
    private bool record = true;

    private float traveledDistance = 0;
    private Vector3 previousLoc;

    private void FixedUpdate()
    {
        if (record)
            RecordDistance();
    }
    private void RecordDistance()
    {
        traveledDistance += Vector3.Distance(transform.position, previousLoc);
        previousLoc = transform.position;
    }
    public void ToggleRecord()
    {
        record = !record;
    }

    public float GetTraveledDistance { get { return traveledDistance; } set { traveledDistance = value; } }

}
