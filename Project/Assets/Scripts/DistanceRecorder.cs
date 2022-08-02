using System;
using System.Collections;
using System.Collections.Generic;
using Dijstra.path;
using ML_Agents.Finder.Scripts;
using UnityEngine;

public class DistanceRecorder : MonoBehaviour
{
    private float traveledDistance = 0;

    public void ToggleRecord()
    {
        // record = !record;
    }
    
    
    //TODO Mode this to agent script
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("spawnArea"))
        {
            traveledDistance++;

            Debug.Log("Agent Traveled Distance : " + traveledDistance + " | passed to =>" + other.gameObject.name);
        }
    }


    public float GetTraveledDistance { get { return traveledDistance; } set { traveledDistance = value; } }

}
