using System;
using System.Collections.Generic;
using System.Linq;
using Dijkstra.Scripts;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.StateMachine;
using ML_Agents.PF.Scripts.UtilsScripts;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ML_Agents.PF.Scripts.RL
{
    public partial class PathFindAgent : Agent
    {

    #region Vars

        [Header("Agent behavior Vars")] [Space]
        [SerializeField] private bool _useVectorObservations = true;

        [Header("Area Vars")] [Space]
        [SerializeField] private PathFindArea _area;
        [SerializeField] private CheckPoint _checkPoint;
        [SerializeField] private Graph _graph;

        private TrainingStateMachine _trainingStateMachine;
        private CountDownTimer _countDownTimer;
        private Coroutine _timerCoroutine; //todo : use it to stop/start coroutine?
        private NodeMapping _nodeMapping;

        private readonly GameObject[] _nodesToFind = new GameObject[2];
        private readonly float[] _nodesDistances = new float[2];
        private GameObject _target;
        private int _targetNodeIndex;

        private Rigidbody _agentRb;

    #endregion

    #region Inits

        public override void Initialize()
        {
            _agentRb = GetComponent<Rigidbody>();

            _trainingStateMachine = GameManager.Instance.CreateStateMachine();

            if (_trainingStateMachine.ConditionsData is null)
            {
                throw new NullReferenceException("Conditions data are null");
            }

            _trainingStateMachine.ConditionsData.MaxStep = MaxStep;
            SetCallBacks();

            _countDownTimer = new CountDownTimer(GameManager.Instance.RewardData.TimerValue, true); //todo : create flag exit
            StartCoroutine(_countDownTimer.IdleMovementCountDown());

            _nodeMapping = new NodeMapping(_area.SpawnAreas);
        }

        private void SetCallBacks()
        {
            _trainingStateMachine.EndEpisodeCallBack += EndEpisode;
            _trainingStateMachine.GiveInternalRewardCallBack += GiveRewardInternal;
            _trainingStateMachine.SwitchTargetNodeCallBack += SwitchTargetNode;
            _trainingStateMachine.UpdateRewardDataStructCallBack += UpdateRewardDataStruct;
        }

    #endregion

    #region Agent

        /// <summary>
        /// what data the ai needs in order to solve the problem
        /// A reasonable approach for determining what information
        /// should be included is to consider what you would need to
        /// calculate an analytical solution to the problem, or what
        /// you would expect a human to be able to use to solve the problem.
        /// </summary>
        /// <param name="sensor"></param>
        public override void CollectObservations(VectorSensor sensor) //12 total
        {
            if (!_useVectorObservations) return;

            sensor.AddObservation(transform.InverseTransformDirection(_agentRb.velocity)); //3
            sensor.AddObservation(_checkPoint.HasPressed); //1   bool //direction to target

            if (!_checkPoint.HasPressed) //check point
            {
                var dir = (_nodesToFind[0].transform.localPosition - transform.localPosition).normalized;
                sensor.AddObservation(new Vector2(dir.x, dir.z)); //2

                if (_trainingStateMachine.ConditionsData.CheckPointPathLength > 0)
                {
                    sensor.AddObservation(_trainingStateMachine.ConditionsData.CheckPointPathLength); //1 float
                }

            }
            else if (_nodesToFind[1].activeInHierarchy) //final goal
            {
                var dir = (_nodesToFind[1].transform.localPosition - transform.localPosition).normalized;
                sensor.AddObservation(new Vector2(dir.x, dir.z)); //2

                if (_trainingStateMachine.ConditionsData.FullPathLength > 0)
                {
                    sensor.AddObservation(_trainingStateMachine.ConditionsData.FullPathLength); //1 float
                }
            }

            sensor.AddObservation(_trainingStateMachine.ConditionsData.StepFactor); //1     float   //the higher the number, the higher the reward
            sensor.AddObservation(_trainingStateMachine.ConditionsData.TraveledDistance); //1   float   //less distance bigger reward

            //_countDownTimer.IsOutOfTime()
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
            else
                discreteActionsOut[0] = 0;
        }

        private void MoveAgent(ActionSegment<int> act)
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
            MoveAgent(actionBuffers.DiscreteActions);
            StepReward();
        }

        public override void OnEpisodeBegin()
        {
            _area.CleanArea();
            _trainingStateMachine.ConditionsData.Reset();

            var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(9);
            var items = enumerable.ToArray();

            var toNodeTransformList = _graph.Nodes.Select(item => item.transform);
            var nodesTransforms = toNodeTransformList as Transform[] ?? toNodeTransformList.ToArray();

            _area.SetNodesPosition(ref nodesTransforms);
            Spawn(items);

            //create the 2D graph
            _graph.ConnectNodes();
            //item[0] => player | item[1] => checkPoint | item[2] => final node
            SetUpPath
            (
                items[(int)IndexofTargetType.Agent],
                items[(int)IndexofTargetType.Check_Point],
                items[(int)IndexofTargetType.Final_Node]
            );

            SetUpDistanceDifferences
            (
                items[(int)IndexofTargetType.Check_Point],
                items[(int)IndexofTargetType.Final_Node]
            );

            ResetTmpVars(items);
        }

    #endregion

    #region OnEpisodeBeginMethods

        private void Spawn(IReadOnlyList<int> items)
        {
            // Debug.Log("#Player# Spawn process...");
            var next = 0;
            //replace agent transform
            _agentRb.velocity = Vector3.zero;
            _area.PlaceNode(gameObject, items[next++]);
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

            //Create CheckPoint and Goal objs, and hide the goal.obj
            _checkPoint.Init(items[next++], items[next++]);

            //Create all the other objects
            for (var i = 0; i < 6; i++)
            {
                _area.CreateBlockNode(items[next++]) /*.Hide(next)*/;
            }
            //nodes transform has updated through the PFArea.cs above(ref nodeTransforms);

            //collect all the data
        }

        private void ResetTmpVars(IReadOnlyList<int> items)
        {
            _target = null;
            _targetNodeIndex = 0;

            _nodesToFind[(int)IndexofTargetType.Agent] = null;
            _nodesToFind[(int)IndexofTargetType.Check_Point] = null;
            _target = _nodesToFind[(int)IndexofTargetType.Agent] = _graph.Nodes[items[(int)IndexofTargetType.Check_Point]].gameObject; //on init target CP
            _nodesToFind[(int)IndexofTargetType.Check_Point] = _graph.Nodes[items[(int)IndexofTargetType.Final_Node]].gameObject; //set final node as second target

            _countDownTimer.TimerValueChanged(GameManager.Instance.RewardData.TimerValue);
            _countDownTimer.StartTimer = true;

            _nodeMapping.Reset();
        }

        private void SetUpPath(int agentIndex, int checkPointIndex, int finalGoalIndex)
        {
            //calculate the distance player - Checkpoint - goal
            _graph.StartNode = _graph.Nodes[agentIndex];
            _graph.CheckPointNode = _graph.Nodes[checkPointIndex];
            _graph.EndNode = _graph.Nodes[finalGoalIndex];

            var pathLen1 = GetShortestPathLength(_graph.StartNode, _graph.CheckPointNode);
            var pathLen2 = GetShortestPathLength(_graph.CheckPointNode, _graph.EndNode);
            var fullLength = pathLen1 + pathLen2;

            _trainingStateMachine.ConditionsData.CheckPointPathLength = pathLen1;
            _trainingStateMachine.ConditionsData.FullPathLength = fullLength;
            // Debug.Log($"#Player#{pathLen1} + {pathLen2} = {fullLength}");
        }

        private int GetShortestPathLength(Node from, Node to)
        {
            var path = _graph.GetShortestPath(from, to);

            if (path.Length <= 0) return -1;

            return (int)path.Length;
        }

        private void SwitchTargetNode()
        {
            _target = _nodesToFind[_targetNodeIndex];
        }

        private void SetUpDistanceDifferences(int nCheckPoint, int nFinalGoal)
        {
            //use for the sharped RF , distance/reward for each target
            //get the distance from agent to cp
            _nodesDistances[0] = Utils.GetDistanceDifference(gameObject, _graph.Nodes[nCheckPoint].gameObject);
            //get the distance from cp to final goal
            _nodesDistances[1] = Utils.GetDistanceDifference(_graph.Nodes[nCheckPoint].gameObject, _graph.Nodes[nFinalGoal].gameObject);
        }

    #endregion

    #region Collition Methods

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("spawnArea")) return;

            _trainingStateMachine.ConditionsData.TraveledDistance++;
            _countDownTimer.TimerValueChanged(GameManager.Instance.RewardData.TimerValue);

            //todo: create script for spawn area, and link the node on this index.area?

            var visitedNodeResult = _nodeMapping.CheckMap(other.gameObject); //todo : create mechanics for visited nodes
            Debug.Log("moved to new node " + other.gameObject.name + " result : " + visitedNodeResult);

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("wall")) { OnHarmfulCollision(); }

            if (collision.gameObject.CompareTag("switchOff")) { OnCheckPointAchieved(); }

            if (collision.gameObject.CompareTag("goal")) { OnFinalGoalAchieved(); }
        }

    #endregion

    #region Reward State Methods

        private void OnHarmfulCollision()
        {
            _trainingStateMachine.ConditionsData.HasTouchedWall = true;
            _trainingStateMachine.RunOnHarmfulCollision();
        }

        private void OnCheckPointAchieved()
        {
            _trainingStateMachine.ConditionsData.HasFoundCheckpoint = true;
            _targetNodeIndex++;

            _trainingStateMachine.RunOnCheckPointReward();
        }

        private void OnFinalGoalAchieved()
        {
            _trainingStateMachine.ConditionsData.HasFoundGoal = true;
            _trainingStateMachine.RunOnFinalGoalReward();
        }

        private void StepReward()
        {
            _trainingStateMachine.ConditionsData.StepCount = StepCount;

            if (_trainingStateMachine.GetType() == typeof(SimpleTraining))
            {
                ((SimpleTraining)_trainingStateMachine).RunOnStepReward();
            }
            else if (_trainingStateMachine.GetType() == typeof(AdvancedTraining))
            {
                ((AdvancedTraining)_trainingStateMachine).RunOnStepReward();
            }

            _trainingStateMachine.IsOutOfTime = _countDownTimer.IsOutOfTime();
        }

        private void GiveRewardInternal(RewardUseType useRewardType, float extraRewardValue)
        {
            switch (useRewardType)
            {
                //Use AddReward() to accumulate rewards between decisions.
                case RewardUseType.Add_Reward:
                    AddReward(extraRewardValue);

                    break;
                //Use SetReward() to overwrite any previous rewards accumulate between decisions.
                case RewardUseType.Set_Reward:
                    SetReward(extraRewardValue);

                    break;
            }
        }

    #endregion

    #region UpdateData

        private void UpdateRewardDataStruct()
        {
            _trainingStateMachine.RewardDataStruct.SetData(
                _trainingStateMachine.HasEpisodeEnded(),
                _trainingStateMachine.FinalRewardConditions.ToArray(),
                Utils.GetDistanceDifference(gameObject, _target),
                _nodesDistances[_targetNodeIndex]
            );
        }

    #endregion
    }

}
