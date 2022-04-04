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
    Path path = new Path();
    private static float episodeCounter = 0;
    private float pLength = 0;
    //private float mLength = 0;
    private bool hasFindGoal = false;
    private bool isFirstTake = true;


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
        //send any data from the previous round before reseting them
        SendData();

        #region Spawning
        //create a array with random unique values with range 0 to 9
        var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(9);
        var items = enumerable.ToArray();
        //holds the count of spawned objects
        int next = 0;

        //whenever the episodes begin, destroy precreated objs
        m_MyArea.CleanArea();       

        
        Transform[] nodeTransforms = new Transform[m_Graph.nodes.Count];
        //nodeTransforms = m_Graph.nodes.ConvertAll<Transform>(Converter<Node,Transform>(Transform t));
        //copy nodes.transform to new array to use that array by reference below
        for (int i = 0; i < m_Graph.nodes.Count; i++)       
            nodeTransforms[i] = m_Graph.nodes[i].transform;

        //pass the array by reference to modify directly within PFArea.cs
        m_MyArea.SetNodesPosition(ref nodeTransforms);

        //replace agent tranform
        m_AgentRb.velocity = Vector3.zero;
        m_MyArea.PlaceObject(gameObject, items[next++]); 
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        //Create CheckPoint and Goal objs, and hide the goal.obj
        m_SwitchLogic.ResetSwitch(items[next++], items[next++]);  

        //Create all the other objects
        for (int i = 0; i < 6; i++)
            m_MyArea.CreateBlockObject(1, items[next++]);

        //nodes transform has updated through the PFArea.cs above(ref nodeTransforms)
        //create's the 2D graph 
        m_Graph.ConnectNodes();
        #endregion

        //calculate the distance player - Checkpoint - goal
        //set start-end nodes
        // _pd = path distance calculated after running dijkstra
        // _ps = list of the nodes in the shortest path
        float _pd1 = CalculateShortestPathLength(m_Graph.nodes[items[0]] , m_Graph.nodes[items[1]]);
        string _ps1 = path.ToString();
        float _pd2 = CalculateShortestPathLength(m_Graph.nodes[items[1]], m_Graph.nodes[items[2]]);
        string _ps2 = path.ToString();

        _pd1 = _pd1 + _pd2;
        _ps1 = _ps1 + " -|- " + _ps2;
        Debug.Log("Path Length : " + _pd1 + "\t ==" + _ps1);

        episodeCounter++;
        //mLength = 0;
        GetComponent<DistanceRecorder>().totalDistance = 0;
        pLength = _pd1;
        hasFindGoal = false;
        isFirstTake = false;
        //Debug.Log(episodeCounter);
    }

    float CalculateShortestPathLength(Node from , Node to)
    {
        //m_Graph.m_Start = from;
        //m_Graph.m_End = to;
        path = m_Graph.GetShortestPath(from, to);
        //Debug.Log(path.length + " --- " + path.ToString());

        if (path.length <= 0)
            Debug.LogError("Path length <= 0");

        return path.length;
    }

    void SendData()
    {
        if(!isFirstTake)
            GameManager.instance.WriteData(episodeCounter, GetComponent<DistanceRecorder>().totalDistance, pLength, hasFindGoal, 0);
    }

    float CalculateRewards(bool hasFindGoal , bool hasFindCp , bool isOnPath)
    {
        //if agent completed the episode succefuly get max reward
        if (hasFindGoal && isOnPath)
            return 2f;
        else if (hasFindGoal && !isOnPath)
            return 1.5f;
        //half way there
        else if (hasFindCp && isOnPath)
            return 1f;
        else if (hasFindCp && !isOnPath)
            return 0.5f;



        return 0;
    }

    void DemoReward()
    {

    }

    void SparseReward()
    {

    }

    void ShapedReward()
    {
        //m_Reward = 0;
    }

    void TerminalConditions()
    {
        //timi limits
        //positive terminals
        //negative terminals
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            hasFindGoal = true;
            SetReward(2f);
            EndEpisode();
        }
    }
}
