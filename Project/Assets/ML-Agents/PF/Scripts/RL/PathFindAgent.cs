using System;
using System.Collections.Generic;
using System.Linq;
using Dijkstra.Scripts;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.Structs;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ML_Agents.PF.Scripts.RL
{
    public class PathFindAgent : Agent
    {

    #region Vars

        [Header("Agent behavior Vars")] [Space]
        [SerializeField] private bool _useVectorObservations = true;

        [Header("Area Vars")] [Space]
        [SerializeField] private PathFindArea _area;
        [SerializeField] private CheckPoint _checkPoint;
        [SerializeField] private Graph _graph;

        private TrainingStateMachine.TrainingStateMachine _trainingStateMachine;

        private readonly GameObject[] _nodesToFind = new GameObject[2];
        private readonly float[] _nodesDistances = new float[2];
        private GameObject _target;
        private int _targetNodeIndex;

        private Rigidbody _agentRb;

        private RewardDataStruct _rewardDataStruct; //move this

    #endregion

    #region Inits

        public override void Initialize()
        {
            _agentRb = GetComponent<Rigidbody>();

            //TODO : error , each agent should have its own state machine?
            _trainingStateMachine = GameManager.Instance.TrainingStateMachine; //get it from the game manager

            if (_trainingStateMachine is null)
            {
                throw new NullReferenceException("State Machine is null");
            }
            if (_trainingStateMachine.ConditionsData is null)
            {
                throw new NullReferenceException("Conditions data are null");
            }

            SetCallBacks();
        }

        private void SetCallBacks()
        {
            _trainingStateMachine.EndEpisodeCallBack += EndEpisode;
            _trainingStateMachine.GiveInternalRewardCallBack += GiveRewardInternal;
            _trainingStateMachine.SwitchTargetNodeCallBack += SwitchTargetNode;
            _trainingStateMachine.UpdateRewardDataWrapperCallBack += UpdateRewardDataWrapper;
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
        public override void CollectObservations(VectorSensor sensor)
        {
            if (!_useVectorObservations) return;

            sensor.AddObservation(transform.InverseTransformDirection(_agentRb.velocity)); //3
            //cp
            sensor.AddObservation(_checkPoint.GetState); //1
            var dir = (_nodesToFind[0].transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dir.x); //1
            sensor.AddObservation(dir.z); //1

            //if goal is active in scene :
            sensor.AddObservation(_nodesToFind[1].activeInHierarchy ? 1 : 0); //1

            if (_nodesToFind[1].activeInHierarchy)
            {
                var dirToGoal = (_nodesToFind[1].transform.localPosition - transform.localPosition).normalized;
                sensor.AddObservation(dirToGoal.x); //1
                sensor.AddObservation(dirToGoal.z); //1
            }
            else
            {
                sensor.AddObservation(0); //x
                sensor.AddObservation(0); //z
            }

            //note : maybe add the Nodes dijkstra
            sensor.AddObservation(_trainingStateMachine.ConditionsData.StepFactor); //1
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

            _rewardDataStruct = new RewardDataStruct();
            _trainingStateMachine.ConditionsData.MaxStep = MaxStep;
        }

        private void ResetTmpVars(IReadOnlyList<int> items)
        {
            _trainingStateMachine.ConditionsData.Reset();

            _target = null;
            _targetNodeIndex = 0;

            _nodesToFind[(int)IndexofTargetType.Agent] = null;
            _nodesToFind[(int)IndexofTargetType.Check_Point] = null;
            _target = _nodesToFind[(int)IndexofTargetType.Agent] = _graph.Nodes[items[(int)IndexofTargetType.Check_Point]].gameObject; //on init target CP
            _nodesToFind[(int)IndexofTargetType.Check_Point] = _graph.Nodes[items[(int)IndexofTargetType.Final_Node]].gameObject; //set final node as second target
        }

        private void SetUpPath(int agentIndex, int checkPointIndex, int finalGoalIndex)
        {
            //calculate the distance player - Checkpoint - goal
            _graph.StartNode = _graph.Nodes[agentIndex];
            _graph.CheckPointNode = _graph.Nodes[checkPointIndex];
            _graph.EndNode = _graph.Nodes[finalGoalIndex];

            var pathLen1 = GetShortestPathLength(_graph.StartNode, _graph.CheckPointNode);
            var pathLen2 = GetShortestPathLength(_graph.CheckPointNode, _graph.EndNode);
            var tmp = pathLen1 + pathLen2;
            _trainingStateMachine.ConditionsData.CheckPointLength = pathLen1;
            _trainingStateMachine.ConditionsData.PathTotalLength = tmp;
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
            _nodesDistances[0] = Utils.Utils.GetDistanceDifference(gameObject, _graph.Nodes[nCheckPoint].gameObject);
            //get the distance from cp to final goal
            _nodesDistances[1] = Utils.Utils.GetDistanceDifference(_graph.Nodes[nCheckPoint].gameObject, _graph.Nodes[nFinalGoal].gameObject);
        }

    #endregion

    #region Collition Methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("spawnArea"))
            {
                _trainingStateMachine.ConditionsData.TraveledDistance++;
                // Debug.Log($"- Agent Distance : {_traveledDistance} | Current node =>{other.gameObject.name}");
            }
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
            _trainingStateMachine.RunOnStepReward();
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

        private void UpdateRewardDataWrapper()
        {
            _rewardDataStruct.CurrentDistance = Utils.Utils.GetDistanceDifference(gameObject, _target);
            _rewardDataStruct.CurrentTargetDistance = _nodesDistances[_targetNodeIndex];
            _rewardDataStruct.HasEpisodeEnd = _trainingStateMachine.HasEpisodeEnded();
            _rewardDataStruct.Conditions = _trainingStateMachine.RewardConditions.ToArray();

            _trainingStateMachine.RewardDataStruct = _rewardDataStruct;
        }


    #endregion
    }

}
