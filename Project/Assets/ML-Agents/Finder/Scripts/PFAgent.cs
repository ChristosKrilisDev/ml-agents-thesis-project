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
    [Space]
    [SerializeField] private bool useVectorObs;

    [Header("Area Vars")]
    [SerializeField] private PFArea _area;
    [SerializeField] private CheckPoint _checkPoint;
    [SerializeField] private Graph _graph;
    private Path _path = new Path();
    private float _pathTotalLength = 0;

    private Rigidbody _agentRb;

    // record data
    //struct MyStruct
    //{
    //    public bool _done;
    //    public int x;
    //}
    DistanceRecorder _distanceRecorder;
    private bool _isFirstTake = false;
    private static float episodeCounter = 0;
    private bool _hasTouchedTheWall = false;
    private bool _hasFindGoal = false;
    private bool _hasFindCP;


    [Header("Reward Function Vars")]
    [SerializeField] private float distanceReward = 0;
    [SerializeField] private readonly float epsilon = 0.4f;
    [SerializeField] private float boostReward = 1;
    [SerializeField] private float _stepFactor;

    private GameObject[] _goalsToFind = new GameObject[2];
    private float[] _goalDistances = new float[2];
    private GameObject _targetGoal;
    private int _targetGoalIndex = 0;

    //timed //step is running 50 times/sec
    //3000 max steps/ 5 decision interv = 600steps per episode
    //Timed Frames
    private int _frameCount = 0;
    private readonly int frameAmmount = 50;

    private enum Indexof
    {
        _Agent = 0,
        _CheckPoint = 1,
        _FinalNode = 2
    }

    #endregion

    #region Agent
    public override void Initialize()
    {
        _agentRb = GetComponent<Rigidbody>();
        _distanceRecorder = GetComponent<DistanceRecorder>();
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
            sensor.AddObservation(transform.InverseTransformDirection(_agentRb.velocity)); //3
            //cp
            sensor.AddObservation(_checkPoint.GetState); //1
            Vector3 dir = (_goalsToFind[0].transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dir.x); //1
            sensor.AddObservation(dir.z); //1


            //if goal is active in scene : 
            sensor.AddObservation(_goalsToFind[1].activeInHierarchy ? 1 : 0);  //1
            if (_goalsToFind[1].activeInHierarchy)
            {
                Vector3 dirToGoal = (_goalsToFind[1].transform.localPosition - transform.localPosition).normalized;
                sensor.AddObservation(dirToGoal.x); //1
                sensor.AddObservation(dirToGoal.z); //1
            }
            else
            {
                sensor.AddObservation(0);//x
                sensor.AddObservation(0);//z
            }

            //note : maybe add the Nodes dijstra
            sensor.AddObservation(_stepFactor);//1
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
        _agentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (--_frameCount <= 0)//O(1) O(n) //TODO HERE
        {

            _frameCount = frameAmmount;
            AddReward(RewardFunction(GetCurrentDistanceDiffrence(this.gameObject, _targetGoal)));//step is running 50 times/sec
            //Debug.Log("---Timed---");
        }
        //SetReward(RewardFunction(DistanceDiffrence(this.gameObject, f_goal)));//step is running 50 times/sec
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
            discreteActionsOut[0] = 3;
        else if (Input.GetKey(KeyCode.W))
            discreteActionsOut[0] = 1;
        else if (Input.GetKey(KeyCode.A))
            discreteActionsOut[0] = 4;
        else if (Input.GetKey(KeyCode.S))
            discreteActionsOut[0] = 2;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            _hasTouchedTheWall = true;
            OnTerminalCondition(-0.25f, true);
        }
        if (collision.gameObject.CompareTag("switchOff"))
        {
            _targetGoalIndex++;
            _hasFindCP = true;
            //SetReward(+1);
            AddReward(2f);
            SwitchTargetToFinalNode();
        }
        if (collision.gameObject.CompareTag("goal"))
        {
            _hasFindGoal = true;
            OnTerminalCondition(2, true);
        }
    }

    private void OnTerminalCondition(float reward, bool useAddReward)
    {
        if (useAddReward) AddReward(reward);
        else SetReward(reward);

        //this will give end reward and end episode
        SetReward(RewardFunction(GetCurrentDistanceDiffrence(this.gameObject, _targetGoal.gameObject)));
    }

    public override void OnEpisodeBegin()
    {
        SendData();

        _area.CleanArea();

        var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(9);
        var items = enumerable.ToArray();

        Transform[] nodesTransforms = new Transform[_graph.nodes.Count];
        Array.Copy(_graph.nodes.ToArray(), nodesTransforms, _graph.nodes.Count);

        _area.SetNodesPosition(ref nodesTransforms);
        Spawn(items);

        //create's the 2D graph 
        _graph.ConnectNodes();
        //item[0] => player | item[1] => checkPoint | item[2] => final node
        SetUpPath(items[(int)Indexof._Agent], items[(int)Indexof._CheckPoint], items[(int)Indexof._FinalNode]);
        SetUpDistanceDifrences(items[(int)Indexof._CheckPoint], items[(int)Indexof._FinalNode]);
        ResetTmpVars(items);
    }
    #endregion

    #region OnEpisodeBeginMethods
    private void Spawn(int[] items)
    {
        int next = 0;
        //replace agent tranform
        _agentRb.velocity = Vector3.zero;
        _area.PlaceNode(this.gameObject, items[next++]);
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        //Create CheckPoint and Goal objs, and hide the goal.obj
        _checkPoint.Init(items[next++], items[next++]);

        //Create all the other objects
        for (int i = 0; i < 6; i++)
            _area.CreateBlockNode(items[next++])/*.Hide(next)*/;

        //nodes transform has updated through the PFArea.cs above(ref nodeTransforms)
    }

    private void ResetTmpVars(int[] items)
    {
        episodeCounter++;
        _distanceRecorder.GetTraveledDistance = 0;
        _stepFactor = 0;
        _hasFindGoal = _hasFindCP = _hasTouchedTheWall = false;
        _targetGoalIndex = _frameCount = 0;

        _targetGoal = _goalsToFind[(int)Indexof._Agent] = _graph.nodes[items[(int)Indexof._CheckPoint]].gameObject; //on init target CP 
        _goalsToFind[(int)Indexof._CheckPoint] = _graph.nodes[items[(int)Indexof._FinalNode]].gameObject;    //set final node as secondtarger
    }

    private void SetUpPath(int n_agent, int n_checkPoint, int n_finalGoal)
    {    //calculate the distance player - Checkpoint - goal
        {
            float p_len1 = AddShortestPathLength(_graph.nodes[n_agent], _graph.nodes[n_checkPoint]);
            float p_len2 = AddShortestPathLength(_graph.nodes[n_checkPoint], _graph.nodes[n_finalGoal]);
            p_len1 += p_len2;
            _pathTotalLength = p_len1;
        }
    }

    private float AddShortestPathLength(Node from, Node to)
    {
        _path = _graph.GetShortestPath(from, to);

        if (_path.length <= 0)
            Debug.LogError("Path length <= 0");

        return _path.length;
    }

    private void SwitchTargetToFinalNode() => _targetGoal = _goalsToFind[_targetGoalIndex];

    private void SetUpDistanceDifrences(int n_checkPoint, int n_finalGoal)
    {
        //use for the sharped RF , distance/reward for each targert
        //get the distance from agent to cp
        _goalDistances[0] = GetCurrentDistanceDiffrence(this.gameObject, _graph.nodes[n_checkPoint].gameObject);
        //get the distance from agent to final goal
        _goalDistances[1] = GetCurrentDistanceDiffrence(_graph.nodes[n_checkPoint].gameObject, _graph.nodes[n_finalGoal].gameObject);
    }
    #endregion

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
        _stepFactor = Math.Abs(this.StepCount - this.MaxStep) / (float)this.MaxStep;
        //Debug.LogFormat("{0} / {1} = {2} ", Mathf.Abs(this.StepCount - this.MaxStep) , MaxStep , stepFactor );


        #region TerminalRewards
        if (HasEpisodeEnded()) //TC1 when it ends? : max step reached or completed task
        {
            if (_hasFindCP)
            {
                if (_hasFindGoal)
                {
                    if (IsDistanceLessThanDijstra())
                    {
                        calculateReward = Math.Abs(boostReward + _stepFactor); //1 + SF 
                        //Debug.Log("Phase : ALl true \t reward : " + calculateReward);
                    }
                    else
                    {
                        calculateReward = -boostReward / 4; //-0.25f
                        //Debug.Log("Phase : Distance is more than dijstrta * 2 \t reward : " + calculateReward);
                    }
                }
                else
                {
                    calculateReward = -boostReward * 3; //-0.5f
                    //Debug.Log("Phase : Didnt find goal \t reward : " + calculateReward);
                }
            }
            else
            {
                calculateReward = -boostReward * 4;// -1 wrost senario
                //Debug.Log("Phase : Didnt CP goal \t reward : " + calculateReward);
            }

            EndEpisode();
        }
        #endregion
        else //encourage agent to keep searing 
        {
            distanceReward = 1 - Mathf.Pow(currDistance / _goalDistances[_targetGoalIndex], epsilon);

            //TODO : 0/100 ??!!
            calculateReward = (distanceReward + 0.0001f / 100) * 0.3f;//50% less //reward a very small ammount, to guide the agent but not big enough to create a looped reward(circle).
            //Debug.LogFormat("Phase : Encourage \t reward : {0}  | target {1}", calculateReward, goalDistances[goalIndex]);
        }


        Debug.LogFormat("Agent :{0} ended with reward = {1}", this.gameObject.name, calculateReward);
        return calculateReward;
        //Use AddReward() to accumulate rewards between decisions.
        //Use SetReward() to overwrite any previous rewards accumulate between decisions.
    }

    bool HasEpisodeEnded()
    {
        return _hasFindGoal || StepCount == MaxStep || _hasTouchedTheWall;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if the distance that agent did is less than dijstras shortest path length</returns>
    bool IsDistanceLessThanDijstra()
    {
        return _distanceRecorder.GetTraveledDistance <= _pathTotalLength * 2;
    }

    /// <summary>
    /// used to find the distance between agent and current goal
    /// </summary>
    /// <param name="pointA">Agent</param>
    /// <param name="pointB">goal</param>
    /// <returns>the distance in float, from pointA to pointB</returns>
    float GetCurrentDistanceDiffrence(GameObject pointA, GameObject pointB)
    {
        if (pointA == null || pointB == null)
            return 0;
        Vector3 _pointA = new Vector3(pointA.transform.localPosition.x, 0, pointA.transform.localPosition.z);
        Vector3 _pointB = new Vector3(pointB.transform.localPosition.x, 0, pointB.transform.localPosition.z);

        return Vector3.Distance(_pointA, _pointB);
    }
    #endregion


    private void SendData()
    {
        if (!_isFirstTake) //dont send data if it's the first episode
        {
            _isFirstTake = false;
            GameManager.instance.WriteData(episodeCounter, GetComponent<DistanceRecorder>().GetTraveledDistance, _pathTotalLength, _hasFindGoal, 0);
        }
    }
}

