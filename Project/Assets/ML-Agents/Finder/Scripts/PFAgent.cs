using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using Dijstra.path;


public class PFAgent : Agent
{
    public GameObject area;
    PFArea m_MyArea;
    Rigidbody m_AgentRb;
    CheckPoint m_SwitchLogic;
    public GameObject areaSwitch;
    public bool useVectorObs;

    public Graph m_Graph;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<PFArea>();
        m_SwitchLogic = areaSwitch.GetComponent<CheckPoint>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(m_SwitchLogic.GetState());
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
        }
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }



    public override void OnEpisodeBegin()
    {
        var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(9);
        var items = enumerable.ToArray();
        int next = 0;

        m_MyArea.CleanArea();


        Transform[] nodeTransforms = new Transform[m_Graph.nodes.Count];
        for (int i = 0; i < m_Graph.nodes.Count; i++)
            nodeTransforms[i] = m_Graph.nodes[i].transform;

        m_MyArea.SetNodesPosition(nodeTransforms);


        m_AgentRb.velocity = Vector3.zero;
        m_MyArea.PlaceObject(gameObject, items[next++]); //place agent

        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        m_SwitchLogic.ResetSwitch(items[next++], items[next++]);  //place cp and send pos for goal


        //rest are blocks
        for (int i = 0; i < 6; i++)
            m_MyArea.CreateBlockObject(1, items[next++]);

        //m_MyArea.CreateBlockObject(1, items[next++]);
        //m_MyArea.CreateBlockObject(1, items[next++]);
        //m_MyArea.CreateBlockObject(1, items[next++]);
        //m_MyArea.CreateBlockObject(1, items[next++]);
        //m_MyArea.CreateBlockObject(1, items[next++]);
        //m_MyArea.CreateBlockObject(1, items[next++]);



        //after all objects are placed, gather the new node pos
        nodeTransforms = m_MyArea.GetNodesNewPosition();

        // set the new pos of the nodes
        for (int i = 0; i < m_Graph.nodes.Count; i++)
            m_Graph.nodes[i].transform.position = nodeTransforms[i].transform.position;

        m_Graph.ConnectNodes();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            SetReward(2f);
            EndEpisode();
        }
    }
}
