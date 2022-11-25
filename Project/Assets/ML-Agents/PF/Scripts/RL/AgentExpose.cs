using System.Collections.Generic;
using Dijkstra.Scripts;
using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.Enums;
using ML_Agents.PF.Scripts.StateMachine;
using ML_Agents.PF.Scripts.Structs;
using UnityEngine;

namespace ML_Agents.PF.Scripts.RL
{

#if UNITY_EDITOR

    public partial class PathFindAgent
    {
        private PathFindAgent _pathFindAgent;
        // private TrainingStateMachine _trainingStateMachine;

        public class AgentExpose
        {
            private readonly PathFindAgent _agent;


            public AgentExpose(PathFindAgent agent)
            {
                _agent = agent;

            }

            public PhaseType PhaseType => GameManager.Instance.PhaseType;
            public TrainingType TrainingType => GameManager.Instance.TrainingType;

            public string AgentName => _agent.name;
            public PathFindArea Area => _agent._area;
            public Graph Graph  => _agent._graph;
            public TrainingStateMachine TrainingStateMachine => _agent._trainingStateMachine;
            public GameObject CurrentTarget => _agent._target;
            public int TargetIndex => _agent._targetNodeIndex;
            public GameObject[] NodesToFind => _agent._nodesToFind;
            public RewardDataStruct RewardDataStruct => _agent._trainingStateMachine.RewardDataStruct;
            public List<bool> EndEpisodeConditions => _agent._trainingStateMachine.EndEpisodeConditions;
            public List<bool> FinalRewardConditions => _agent._trainingStateMachine.FinalRewardConditions;
            public ConditionsData ConditionsData => TrainingStateMachine.ConditionsData;


        }
    }
#endif
}

