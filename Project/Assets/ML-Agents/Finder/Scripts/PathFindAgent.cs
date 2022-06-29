using System;
using System.Collections.Generic;
using System.Linq;
using Dijstra.path;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


namespace ML_Agents.Finder.Scripts
{
    public class PathFindAgent : Agent
    {

#region Vars

        [Header("Agent behavior Vars")] [Space]
        [SerializeField]
        private bool _useVectorObs;

        [Header("Area Vars")]
        [SerializeField] private PathFindArea _area;
        [SerializeField] private CheckPoint _checkPoint;
        [SerializeField] private Graph _graph;
        private Path _path = new Path();
        private float _pathTotalLength;

        private Rigidbody _agentRb;

        private DistanceRecorder _distanceRecorder;
        // private static float _episodeCounter;


        private bool _hasTouchedTheWall;
        private bool _hasFoundGoal;
        private bool _hasFoundCheckpoint;
        private float _stepFactor;

        private readonly GameObject[] _nodesToFind = new GameObject[2];
        private readonly float[] _nodesDistances = new float[2];
        private GameObject _targetObjectToFind;
        private int _findTargetNodeIndex;
        
        private static PathFindAgent _masterInit;

        private enum Indexof
        {
            AGENT = 0,
            CHECK_POINT = 1,
            FINAL_NODE = 2
        }
        private enum Use
        {
            NONE,
            ADD_REWARD,
            SET_REWARD
        }

#endregion

#region Agent

        public override void Initialize()
        {
            _agentRb = GetComponent<Rigidbody>();
            _distanceRecorder = GetComponent<DistanceRecorder>();

            if (_masterInit) return;
            _masterInit = this;
            RewardFunction.MaxStep = MaxStep;
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
            _stepFactor = Math.Abs(StepCount - MaxStep) / (float)MaxStep;
            GiveRewardInternal(Use.SET_REWARD, CalculateReward());

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
            else
                discreteActionsOut[0] = 0;

        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("wall"))
            {
                _hasTouchedTheWall = true;
                OnTerminalCondition(Use.SET_REWARD, true, -0.25f);
            }
            if (collision.gameObject.CompareTag("switchOff"))
            {
                _findTargetNodeIndex++;
                _hasFoundCheckpoint = true;
                GiveRewardInternal(Use.ADD_REWARD, 2);
                SwitchTargetToFinalNode();
            }
            if (collision.gameObject.CompareTag("goal"))
            {
                _hasFoundGoal = true;
                GiveRewardInternal(Use.ADD_REWARD, 2);
                OnTerminalCondition(Use.SET_REWARD);
            }
        }

        public override void OnEpisodeBegin()
        {
            _area.CleanArea();

            var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(9);
            var items = enumerable.ToArray();

            var toNodeTransformList = _graph.nodes.Select(item => item.transform);
            var nodesTransforms = toNodeTransformList as Transform[] ?? toNodeTransformList.ToArray();

            _area.SetNodesPosition(ref nodesTransforms);
            Spawn(items);

            //create the 2D graph
            _graph.ConnectNodes();
            //item[0] => player | item[1] => checkPoint | item[2] => final node
            SetUpPath
            (
                items[(int)Indexof.AGENT],
                items[(int)Indexof.CHECK_POINT],
                items[(int)Indexof.FINAL_NODE]
            );

            SetUpDistanceDifferences
            (
                items[(int)Indexof.CHECK_POINT],
                items[(int)Indexof.FINAL_NODE]
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
                _area.CreateBlockNode(items[next++]) /*.Hide(next)*/;

            //nodes transform has updated through the PFArea.cs above(ref nodeTransforms)
        }

        private void ResetTmpVars(IReadOnlyList<int> items)
        {
            _stepFactor = 0;
            _findTargetNodeIndex = 0;
            _distanceRecorder.GetTraveledDistance = 0;
            _hasFoundGoal = _hasFoundCheckpoint = _hasTouchedTheWall = false;
            //_findTargetNodeIndex = _frameCount = 0;
            _targetObjectToFind = _nodesToFind[(int)Indexof.AGENT] = _graph.nodes[items[(int)Indexof.CHECK_POINT]].gameObject; //on init target CP
            _nodesToFind[(int)Indexof.CHECK_POINT] = _graph.nodes[items[(int)Indexof.FINAL_NODE]].gameObject; //set final node as second target
        }

        private void SetUpPath(int nAgent, int nCheckPoint, int nFinalGoal)
        {
            //calculate the distance player - Checkpoint - goal
            {
                //visual tool
                _graph.m_Start = _graph.nodes[nAgent];
                _graph.m_CheckPoint = _graph.nodes[nCheckPoint];
                _graph.m_End = _graph.nodes[nFinalGoal];
                
                var pLen1 = AddShortestPathLength(_graph.nodes[nAgent], _graph.nodes[nCheckPoint]);
                var pLen2 = AddShortestPathLength(_graph.nodes[nCheckPoint], _graph.nodes[nFinalGoal]);
                pLen1 += pLen2;
                _pathTotalLength = pLen1;
            }
        }

        private float AddShortestPathLength(Node from, Node to)
        {
            _path = _graph.GetShortestPath(from, to);
            if (_path.length <= 0) return -1;
            return _path.length;
        }

        private void SwitchTargetToFinalNode()
        {
            _targetObjectToFind = _nodesToFind[_findTargetNodeIndex];
        }

        private void SetUpDistanceDifferences(int nCheckPoint, int nFinalGoal)
        {
            //use for the sharped RF , distance/reward for each target
            //get the distance from agent to cp
            _nodesDistances[0] = EpisodeHandler.GetDistanceDifference(gameObject, _graph.nodes[nCheckPoint].gameObject);
            //get the distance from agent to final goal
            _nodesDistances[1] = EpisodeHandler.GetDistanceDifference(_graph.nodes[nCheckPoint].gameObject, _graph.nodes[nFinalGoal].gameObject);
        }

    #endregion

#region RewardMethods

        //Use AddReward() to accumulate rewards between decisions.
        //Use SetReward() to overwrite any previous rewards accumulate between decisions.
        private void GiveRewardInternal(Use useRewardType = Use.NONE, float extraRewardValue = 0)
        {
            switch (useRewardType)
            {
                case Use.NONE: //Don't Give reward
                default:
                    break;
                case Use.ADD_REWARD:
                    AddReward(extraRewardValue);
                    break;
                case Use.SET_REWARD:
                    SetReward(extraRewardValue);
                    break;
            }
        }
        private void OnTerminalCondition(Use useTypeReward = Use.NONE, bool useExtraReward = false, float extraRewardValue = 0)
        {
            //give a reward for the last action the agent did
            if (useExtraReward) GiveRewardInternal(useTypeReward, extraRewardValue);

            //get the calculate reward on terminal condition
            GiveRewardInternal(useTypeReward, CalculateReward());

            EndEpisode();
        }
        private float CalculateReward()
        {
            return RewardFunction.GetComplexReward
            (
                EpisodeHandler.GetDistanceDifference(gameObject, _targetObjectToFind),
                _nodesDistances[_findTargetNodeIndex],
                StepCount,
                HasEpisodeEnded(),
                _hasFoundCheckpoint,
                _hasFoundGoal,
                EpisodeHandler.CompareCurrentDistance(_distanceRecorder.GetTraveledDistance, _pathTotalLength, 2)
            );
        }

  #endregion

        private bool HasEpisodeEnded()
        {
            IEnumerable<bool> conditions = new List<bool>
            {
                _hasFoundGoal,
                StepCount == MaxStep,
                _hasTouchedTheWall
            };
            return EpisodeHandler.HasEpisodeEnded(conditions);
        }
    }
}
