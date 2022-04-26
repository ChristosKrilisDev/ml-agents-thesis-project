using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using Dijstra.path;
using System.Collections;

public class PFAgent : Agent
{

    #region Vars
    [Header("Agent behavior Vars")]
    public bool useVectorObs;

    [Header("Area Vars")]
    public GameObject area;
    public GameObject areaSwitch;
    PFArea m_MyArea;
    Rigidbody m_AgentRb;
    CheckPoint m_SwitchLogic;

    public Graph m_Graph;
    Path path = new Path();
    DistanceRecorder m_DistanceRecorder;
    float p_totalLength = 0;

    // record data
    bool isFirstTake = true;
    static float episodeCounter = 0;
    bool hasFindGoal = false;
    bool hasTouchedTheWall = false;

    [Space]
    [Header("Reward Function Vars")]
    [SerializeField]
    [Tooltip("used to create a gradient graph")]
    float epsilon = 0.4f;
    [SerializeField]
    [Tooltip("dicrease the total value of the step-reward by %.{0..1}={0%..100%}")]
    float dP = 0.5f;
    [SerializeField]
    [Tooltip("Only applies when it finish all the tasks. value of 0 will not affect the reward")]
    float rewardBoostHelper = 2;
    [SerializeField]
    [Tooltip("Use it to help the agent take higer reward signals(positive and negative).Value of 0 will not affect the reward")]
    float boostHelper = 3;  //0-5 values, 0 will not affect the reward

    float distanceR = 0;
    float baseBoostReward = 1;
    float stepFuelFactor;

    GameObject[] goalsToFind = new GameObject[2];
    float[] goalDistances = new float[2];
    GameObject f_goal;
    int goalIndex = 0;
    bool hasFindCP;

    //timed //step is running 50 times/sec
    //3000 max steps/ 5 decision interv = 600steps per episode

    int frameCount = 0;
    readonly int frameAmmount = 50;

    int counter = 0;

    #endregion

    #region Agent
    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<PFArea>();
        m_SwitchLogic = areaSwitch.GetComponent<CheckPoint>();
        m_DistanceRecorder = GetComponent<DistanceRecorder>();
    }

    /// <summary>
    /// what data the ai needs in order to solve the problem
    /// A reasonable approach for determining what information
    /// should be included is to consider what you would need to
    /// calculate an analytical solution to the problem, or what
    /// you would expect a human to be able to use to solve the problem.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity)); //3

            //cp
            sensor.AddObservation(m_SwitchLogic.GetState()); //1
            Vector3 dir = (goalsToFind[0].transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dir.x); //1
            sensor.AddObservation(dir.z); //1


            //if goal is active in scene : 
            sensor.AddObservation(goalsToFind[1].activeInHierarchy ? 1 : 0);  //1
            if (goalsToFind[1].activeInHierarchy)
            {
                Vector3 dirToGoal = (goalsToFind[1].transform.localPosition - transform.localPosition).normalized;
                sensor.AddObservation(dirToGoal.x); //1
                sensor.AddObservation(dirToGoal.z); //1
            }
            else
            {
                sensor.AddObservation(0);//x
                sensor.AddObservation(0);//z
            }

            //note : maybe add the Nodes dijstra
            sensor.AddObservation(stepFuelFactor);//1
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
        if (++frameCount >= frameAmmount)//O(1) O(n) //TODO HERE
        {
            frameCount = 0;
            //Debug.Log("---Timed---");
            //set
            AddReward(RewardFunction(DistanceDiffrence(this.gameObject, f_goal)));//step is running 50 times/sec
            counter++;
            Debug.Log(counter);
        }

        //AddReward(RewardFunction(DistanceDiffrence(this.gameObject, f_goal)));//300 max steps = 300 times
        //SetReward(RewardFunction(DistanceDiffrence(this.gameObject, f_goal)));//step is running 50 times/sec
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

        if (collision.gameObject.CompareTag("wall"))
        {
            //SetReward(-0.25f);
            hasTouchedTheWall = true;
            //SetReward(RewardFunction(DistanceDiffrence(this.gameObject, f_goal.gameObject)));//this will give end reward and end episode
        }

        if (collision.gameObject.CompareTag("switchOff")) //collision.gameObject.CompareTag("swichOff")
        {
            //AddReward(+0.5f);

            goalIndex++;
            hasFindCP = true;
            SwitchGoalToFind();
        }

        if (collision.gameObject.CompareTag("goal"))
        {
            //AddReward(+0.5f);
            hasFindGoal = true;
            //SetReward(RewardFunction(DistanceDiffrence(this.gameObject, f_goal.gameObject)));
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

        SetUpPath(items[0], items[1], items[2]);
        SetUpDistanceDifrences(items[1], items[2]);
        ResetTmpVars();
        f_goal = goalsToFind[0] = m_Graph.nodes[items[1]].gameObject;
        goalsToFind[1] = m_Graph.nodes[items[2]].gameObject;
        //SwitchGoalToFind();
    }
    #endregion

    void ResetTmpVars()
    {
        counter = 0;
        //mLength = 0;
        m_DistanceRecorder.GetTraveledDistance = 0;
        episodeCounter++;
        hasFindGoal = false;
        goalIndex = 0;
        frameCount = 0;
        hasFindCP = false;
        stepFuelFactor = 0;
        hasTouchedTheWall = false;
    }

    void SetUpPath(int n_agent, int n_checkPoint, int n_finalGoal)
    {
        //calculate the distance player - Checkpoint - goal
        //set start-end nodes
        // _pd = path distance calculated after running dijkstra
        // _ps = list of the nodes in the shortest path
        float p_len1 = CalculateShortestPathLength(m_Graph.nodes[n_agent], m_Graph.nodes[n_checkPoint]);
        float p_len2 = CalculateShortestPathLength(m_Graph.nodes[n_checkPoint], m_Graph.nodes[n_finalGoal]);
        p_len1 = p_len1 + p_len2;
        p_totalLength = p_len1;

        //string _ps2 = path.ToString();
        //string _ps1 = path.ToString();
        //_ps1 = _ps1 + " -|- " + _ps2;
        //Debug.Log("Path Length : " + _pd1 + "\t ==" + _ps1);
    }
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

    void SwitchGoalToFind()
    {
        f_goal = goalsToFind[goalIndex];
    }

    void SetUpDistanceDifrences(int n_checkPoint, int n_finalGoal)
    {
        //get the distance from agent to cp
        goalDistances[0] = DistanceDiffrence(this.gameObject, m_Graph.nodes[n_checkPoint].gameObject);
        //get the distance from agent to final goal
        goalDistances[1] = DistanceDiffrence(m_Graph.nodes[n_checkPoint].gameObject, m_Graph.nodes[n_finalGoal].gameObject);
    }

    #region RewardFunction

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
        //stepFuelFactor, less steps agent does, more the reward
        stepFuelFactor = Math.Abs(this.StepCount - this.MaxStep) / (float)this.MaxStep;
        //Debug.LogFormat("{0} / {1} = {2} ", Mathf.Abs(this.StepCount - this.MaxStep) , MaxStep , stepFactor );


        #region TerminalRewards
        if (HasEpisodeEnded()) //TC1 when it ends? : max step reached or completed task
        {
            if (hasFindCP)
            {
                if (hasFindGoal)
                {
                    if (IsDistanceLessThanDijstra())
                    {
                        calculateReward = rewardBoostHelper + boostHelper + (baseBoostReward * stepFuelFactor); //1 + SF //2> r > 1
                        //Debug.Log("Phase : ALl true \t reward : " + calculateReward);
                    }
                    else
                    {
                        calculateReward = -(4) * 0.25f; //25%
                        //Debug.Log("Phase : Distance is more than dijstrta * 2 \t reward : " + calculateReward);
                    }
                }
                else
                {
                    calculateReward = -(4) * 0.5f; //50%
                    //Debug.Log("Phase : Didnt find goal \t reward : " + calculateReward);
                }
            }
            else
            {
                calculateReward = -(/*boostHelper + baseBoostReward*/4) * 1f;// 100% -1 wrost senario
                //Debug.Log("Phase : Didnt CP goal \t reward : " + calculateReward);
            }

            EndEpisode();
        }
        #endregion
        else //encourage agent to keep searing 
        {
            distanceR = 1 - Mathf.Pow(currDistance / goalDistances[goalIndex], epsilon);        //-> Values{0..1}^e
            //30% less //reward a very small ammount, to guide the agent but not big enough to create a looped reward(circle).
            calculateReward = (distanceR) * dP;
            //Debug.LogFormat("Phase : Encourage \t reward : {0}  | target {1}", calculateReward, goalDistances[goalIndex]);
        }


        //Debug.LogFormat("Agent :{0} ended with reward = {1}", this.gameObject.GetComponentInParent<GameObject>().name, calculateReward);
        return calculateReward;
        //Use AddReward() to accumulate rewards between decisions.
        //Use SetReward() to overwrite any previous rewards accumulate between decisions.
    }

    bool HasEpisodeEnded()
    {
        return hasFindGoal || StepCount == MaxStep || hasTouchedTheWall;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if the distance that agent did is less than dijstras shortest path length</returns>
    bool IsDistanceLessThanDijstra()
    {
        return m_DistanceRecorder.GetTraveledDistance <= p_totalLength * 2;
    }

    /// <summary>
    /// used to find the distance between agent and current goal
    /// </summary>
    /// <param name="pointA">Agent</param>
    /// <param name="pointB">goal</param>
    /// <returns>the distance in float, from pointA to pointB</returns>
    float DistanceDiffrence(GameObject pointA, GameObject pointB)
    {
        if (pointA == null || pointB == null)
            return 0;
        Vector3 _pointA = new Vector3(pointA.transform.localPosition.x, 0, pointA.transform.localPosition.z);
        Vector3 _pointB = new Vector3(pointB.transform.localPosition.x, 0, pointB.transform.localPosition.z);

        return Vector3.Distance(_pointA, _pointB);
    }
    #endregion

    void SendData()
    {
        if (!isFirstTake)
        {
            isFirstTake = false;
            GameManager.instance.WriteData(episodeCounter, GetComponent<DistanceRecorder>().GetTraveledDistance, p_totalLength, hasFindGoal, 0);
        }
    }
}

