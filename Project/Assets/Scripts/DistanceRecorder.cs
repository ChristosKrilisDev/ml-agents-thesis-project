using System;
using System.Collections;
using System.Collections.Generic;
using Dijstra.path;
using ML_Agents.Finder.Scripts;
using UnityEngine;

public class DistanceRecorder : MonoBehaviour
{
    
    /// <summary>
    /// TODO : REMOVE THIS .CS
    /// </summary>
    private float traveledDistance = 0;

    public void ToggleRecord()
    {
        // record = !record;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("spawnArea"))
            traveledDistance++;
    }


    public float GetTraveledDistance { get { return traveledDistance; } set { traveledDistance = value; } }

}
