using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace ML_Agents.PF.Scripts.TrainingStateMachine
{
    public class AdvancedTraining : TrainingStateMachine
    {

        // private readonly int _maxStep;

        public AdvancedTraining()
        {

        }

        public override void RunStepReward()
        {
            base.RunStepReward();
            // _stepFactor = (_stepFactor - _maxStep) / _maxStep;
            // Debug.Log("agent moved back step factor: " + _stepFactor);
            // GiveRewardInternal(UxmlAttributeDescription.Use.Add_Reward, -_stepFactor);
            //
            // var newStepReward = CalculateReward();
            //
            // if (Utils.Utils.NearlyEqual(_previousStepReward, newStepReward, 0.001f)) return;
            //
            // if (_previousStepReward > newStepReward)
            // {
            //     GiveRewardInternal(UxmlAttributeDescription.Use.Add_Reward, -GameManager.Instance.RewardData.StepReward/10);
            //     return;
            // }
            //
            // _previousStepReward = newStepReward;
            // GiveRewardInternal(UxmlAttributeDescription.Use.Add_Reward, newStepReward);
            // Debug.Log("Step Reward : " + newStepReward);
            //
            // return;
        }

        public override void RunCheckPointReward()
        {
            base.RunCheckPointReward();
            // GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.Reward);
            //
            // if ((int)stateMachine.PhaseType == 1)
            // {
            //     OnTerminalCondition(Use.Set_Reward);
            //     return;
            // }
            //
            // if ((int)stateMachine.PhaseType == 2)
            // {
            //     WriteDijkstraData(_checkPointLength, CP_D_KEY);
            //     OnTerminalCondition(Use.Set_Reward);
            //     return;
            // }
            //
            // if ((int)stateMachine.PhaseType >= 3)
            // {
            //     WriteDijkstraData(_checkPointLength, CP_D_KEY);
            //     GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.Reward);
            //     SwitchTargetNode();
            // }
        }

        public override void RunFinalGoalReward()
        {
            base.RunFinalGoalReward();

            // GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.Reward);
            // Academy.Instance.StatsRecorder.Add("Distance/Distance Traveled", _traveledDistance, StatAggregationMethod.Histogram);
            // Academy.Instance.StatsRecorder.Add("Distance/Shortest Path", _pathTotalLength, StatAggregationMethod.Histogram);
            //
            // if (stateMachine.TrainingType == GameManager.TrainingType.Advanced)
            // {
            //     GiveRewardInternal(Use.Add_Reward, GameManager.Instance.RewardData.Reward);
            //
            //     if ((int)stateMachine.PhaseType == 3)
            //     {
            //         OnTerminalCondition(Use.Set_Reward);
            //     }
            //     else if ((int)stateMachine.PhaseType == 4)
            //     {
            //         WriteDijkstraData(_checkPointLength + _pathTotalLength, FULL_D_KEY);
            //         OnTerminalCondition(Use.Set_Reward);
            //     }
            // }

        }

        public override bool RunHasEpisodeEnded()
        {
            base.RunHasEpisodeEnded();
            //
            // else if (GameManager.Instance._stateMachine.TrainingType == GameManager.TrainingType.Advanced && (int)GameManager.Instance._stateMachine.PhaseType <= 2)
            // {
            //     conditions = new List<bool>
            //     {
            //         _hasFoundCheckpoint,
            //         StepCount == MaxStep,
            //         _hasTouchedWall,
            //         Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _checkPointLength)
            //     };
            //
            // }
            // else if (GameManager.Instance._stateMachine.TrainingType == GameManager.TrainingType.Advanced)
            // {
            //     conditions = new List<bool>
            //     {
            //         _hasFoundGoal,
            //         StepCount == MaxStep,
            //         _hasTouchedWall,
            //         Utils.Utils.CompareCurrentDistanceWithMaxLengthPath(_traveledDistance, _pathTotalLength + _checkPointLength)
            //     };
            //
            // }
            return true;
        }

    }
}
