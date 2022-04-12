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

    #region Vars
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
    DistanceRecorder m_DistanceRecorder;

    private Transform goal;

    #endregion

    #region Agent
    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<PFArea>();
        m_SwitchLogic = areaSwitch.GetComponent<CheckPoint>();
        m_DistanceRecorder = GetComponent<DistanceRecorder>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(m_SwitchLogic.GetState());
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));

            sensor.AddObservation(transform.position.x);
            sensor.AddObservation(transform.position.y);
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
        RewardFunction(DistanceDiffrence(this.transform.position, goal.transform.position));
        //AddReward(-1f / MaxStep);
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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            hasFindGoal = true;
            //SetReward(2f);
            EndEpisode();
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
        float _pd1 = CalculateShortestPathLength(m_Graph.nodes[items[0]], m_Graph.nodes[items[1]]);
        string _ps1 = path.ToString();
        float _pd2 = CalculateShortestPathLength(m_Graph.nodes[items[1]], m_Graph.nodes[items[2]]);
        string _ps2 = path.ToString();

        _pd1 = _pd1 + _pd2;
        _ps1 = _ps1 + " -|- " + _ps2;
        Debug.Log("Path Length : " + _pd1 + "\t ==" + _ps1);

        //set goal to search
        goal = m_Graph.nodes[items[1]].transform;

        episodeCounter++;
        //mLength = 0;
        GetComponent<DistanceRecorder>().totalDistance = 0;
        pLength = _pd1;
        hasFindGoal = false;
        isFirstTake = false;
        //Debug.Log(episodeCounter);
    }
    #endregion


    float CalculateShortestPathLength(Node from, Node to)
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
        if (!isFirstTake)
            GameManager.instance.WriteData(episodeCounter, GetComponent<DistanceRecorder>().totalDistance, pLength, hasFindGoal, 0);
    }


    readonly float epsilon = 0.4f;
    float distanceR = 0;
    float boostReward = 100;

    /// <summary>
    /// 
    /// </summary>
    /// <SharpedRewardFunction> distanceReward = 1 - ( Dx/DijDχ )^(epsilon) </SharpedRewardFunction>
    /// <value distanceReward> The value of the reward based on how close to the target the agent is</value>
    /// <value Dx> the distance between agent and goal in straint line from point A to B </value>
    /// <value DijDx> the min distance (on init) from player pos, to goal </value>
    /// <value epsilon> value to create sharped gradient learning curve </value>
    /// <value maxBonusReward> A huge reward given to the agent when it completes the task </value>
    ///
    /// <input currDistance> The current distance between agent and goal</input>
    /// <input hasEpisodeEnded> Whenever the episodes ends, calculate terminal reward </input>
    /// 
    /// <returns calculateReward> The reward agent will take for that state based based on the reward state</returns>
    public float RewardFunction(float currDistance)
    {
        float calculateReward = 0;
        //agent reward is reseted to 0 after every step
        // Additionally, the magnitude of the reward should not exceed 1.0
        float stepFactor = Math.Abs(this.StepCount - this.MaxStep) / (float)this.MaxStep;
        Debug.LogFormat("{0} / {1} = {2} ", Mathf.Abs(this.StepCount - this.MaxStep) , MaxStep , stepFactor );


        #region TerminalRewards
        if (HasEpisodeEnded()) //when it ends : max step reached or completed task
        {
            if (hasFindGoal)
            {
                if (IsDistanceLessThanDijstra())
                {
                    calculateReward = Math.Abs(boostReward * stepFactor);
                    Debug.Log("Phase : ALl true \t reward : " + calculateReward);
                }
                else
                {
                    calculateReward = -boostReward / 10;
                    Debug.Log("Phase : Distance is more than dijstrta \t reward : " + calculateReward);

                }
            }
            else
            {
                calculateReward = -boostReward;
                Debug.Log("Phase : Didnt find goal \t reward : " + calculateReward);
            }

            EndEpisode();
        }
        #endregion
        else //encourage agent to keep searing 
        {
            distanceR = 1 - Mathf.Pow(currDistance / pLength, epsilon); //change pL to the init distance from spawning
            calculateReward = distanceR * stepFactor;
            //Debug.LogFormat("Phase : Encourage \t reward : {0}", calculateReward);

        }
        return calculateReward;
    }

    bool HasEpisodeEnded()
    {
        return hasFindGoal || StepCount == MaxStep;
    }

    bool IsDistanceLessThanDijstra()
    {
        return m_DistanceRecorder.totalDistance <= pLength;
    }

    float DistanceDiffrence(Vector3 pointA, Vector3 pointB)
    {
        if (pointA == null || pointB == null)
            return 0;

        return Vector3.Distance(pointA, pointB);
    }

}

