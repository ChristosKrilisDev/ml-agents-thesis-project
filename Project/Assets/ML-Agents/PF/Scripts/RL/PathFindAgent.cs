using System;
using System.Collections.Generic;
using System.Linq;
using Dijkstra.Scripts;
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
        [SerializeField] private bool _useVectorObs;

        [Header("Area Vars")] [Space]
        [SerializeField] private PathFindArea _area;
        [SerializeField] private CheckPoint _checkPoint;
        [SerializeField] private Graph _graph;

        private TrainingStateMachine _trainingStateMachine;
        private int _checkPointLength;
        private int _pathTotalLength;
        private float _traveledDistance;

        private Rigidbody _agentRb;

        private RewardDataWrapper _rewardDataWrapper;
        private bool _hasTouchedWall;
        private bool _hasFoundCheckpoint;
        private bool _hasFoundGoal;

        private float _stepFactor;
        private float _previousStepReward;

        private readonly GameObject[] _nodesToFind = new GameObject[2];
        private readonly float[] _nodesDistances = new float[2];
        private GameObject _targetObjectToFind;
        private int _findTargetNodeIndex;


        //move theses
        private const string FULL_D_KEY = "Agent/Full Dijkstra Success Rate";
        private const string CP_D_KEY = "Agent/Check Point Dijkstra Success Rate";

    #endregion

    #region Agent

        public override void Initialize()
        {
            _agentRb = GetComponent<Rigidbody>();
            _trainingStateMachine = new TrainingStateMachine();//get it from the game manager
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
            if (!_useVectorObs) return;

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
            sensor.AddObservation(_stepFactor); //1
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
                items[(int)Indexof.Agent],
                items[(int)Indexof.Check_Point],
                items[(int)Indexof.Final_Node]
            );

            SetUpDistanceDifferences
            (
                items[(int)Indexof.Check_Point],
                items[(int)Indexof.Final_Node]
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
            _rewardDataWrapper = new RewardDataWrapper();
        }

        private void ResetTmpVars(IReadOnlyList<int> items)
        {
            // _pathTotalLength = 0;
            _previousStepReward = 0;
            _findTargetNodeIndex = 0;
            _stepFactor = 0;
            _traveledDistance = -1;
            // _blockStepReward = false;
            _hasFoundGoal = _hasFoundCheckpoint = _hasTouchedWall = false;

            _targetObjectToFind = null;
            _nodesToFind[(int)Indexof.Agent] = null;
            _nodesToFind[(int)Indexof.Check_Point] = null;
            _targetObjectToFind = _nodesToFind[(int)Indexof.Agent] = _graph.Nodes[items[(int)Indexof.Check_Point]].gameObject; //on init target CP
            _nodesToFind[(int)Indexof.Check_Point] = _graph.Nodes[items[(int)Indexof.Final_Node]].gameObject; //set final node as second target
        }

        private void SetUpPath(int agentIndex, int checkPointIndex, int finalGoalIndex)
        {
            //calculate the distance player - Checkpoint - goal
            {
                //visual tool
                _graph.StartNode = _graph.Nodes[agentIndex];
                _graph.CheckPointNode = _graph.Nodes[checkPointIndex];
                _graph.EndNode = _graph.Nodes[finalGoalIndex];

                var pathLen1 = GetShortestPathLength(_graph.StartNode, _graph.CheckPointNode);
                var pathLen2 = GetShortestPathLength(_graph.CheckPointNode, _graph.EndNode);
                var tmp = pathLen1 + pathLen2;
                _checkPointLength = (int)pathLen1; //TODO : make them int
                _pathTotalLength = (int)tmp;

                // Debug.Log($" Min Length : {_pathTotalLength} = {pathLen1} + {pathLen2}");
            }
        }

        private float GetShortestPathLength(Node from, Node to)
        {
            var path = _graph.GetShortestPath(from, to);

            if (path.Length <= 0) return -1;

            return path.Length;
        }

        private void SwitchTargetNode()
        {
            _targetObjectToFind = _nodesToFind[_findTargetNodeIndex];
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("spawnArea"))
            {
                _traveledDistance++;
                // Debug.Log($"- Agent Distance : {_traveledDistance} | Current node =>{other.gameObject.name}");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("wall")) { OnHarmfulCollision(); }

            if (collision.gameObject.CompareTag("switchOff")) { OnCheckPointAchieved(); }

            if (collision.gameObject.CompareTag("goal")) { OnFinalGoalAchieved(); }
        }

    #region RewardMethods

        private void OnHarmfulCollision()
        {
            _hasTouchedWall = true;
            _trainingStateMachine.RunHarmfulCollision();
        }

        private void OnCheckPointAchieved()
        {
            _hasFoundCheckpoint = true;
            _findTargetNodeIndex++;

            _trainingStateMachine.RunCheckPointReward();
        }

        private void OnFinalGoalAchieved()
        {
            _hasFoundGoal = true;
            _trainingStateMachine.RunFinalGoalReward();
        }

        private void StepReward()
        {
            _trainingStateMachine.RunStepReward();
        }


        private void WriteDijkstraData(int length, string key)
        {
            Utils.Utils.WriteDijkstraData(_traveledDistance, length, key);
        }

        //Use AddReward() to accumulate rewards between decisions.
        //Use SetReward() to overwrite any previous rewards accumulate between decisions.
        private void GiveRewardInternal(Use useRewardType, float extraRewardValue)
        {
            switch (useRewardType)
            {
                case Use.Add_Reward:
                    AddReward(extraRewardValue);

                    break;
                case Use.Set_Reward:
                    SetReward(extraRewardValue);

                    break;
            }
        }

        private void OnTerminalCondition(Use useTypeReward)
        {
            GiveRewardInternal(useTypeReward, CalculateReward());
            Debug.Log("Terminal Condition");
            Debug.Log(CalculateReward());
            EndEpisode();
        }

        private float CalculateReward()
        {
            _trainingStateMachine.RunCalculateReward();

            UpdateRewardDataWrapper(conditions.ToArray());

            return RewardFunction.GetComplexReward
            (
                _rewardDataWrapper
            );
        }

    #endregion

        private void UpdateRewardDataWrapper(bool[] conditions)
        {
            _rewardDataWrapper.CurrentDistance = Utils.Utils.GetDistanceDifference(gameObject, _targetObjectToFind);
            _rewardDataWrapper.CurrentTargetDistance = _nodesDistances[_findTargetNodeIndex];
            _rewardDataWrapper.HasEpisodeEnd = HasEpisodeEnded();
            _rewardDataWrapper.Conditions = conditions;
        }

        private bool HasEpisodeEnded()
        {

            IEnumerable<bool> conditions = null;

            _trainingStateMachine.RunHasEpisodeEnded();

            return Utils.Utils.HasEpisodeEnded(conditions);
        }
    }

}
