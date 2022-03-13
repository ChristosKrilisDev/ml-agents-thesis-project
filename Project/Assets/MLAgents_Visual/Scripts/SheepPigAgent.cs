using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class SheepPigAgent : Agent {

    [SerializeField] private SheepPigEnvironment sheepPigEnvironment;

    public override void OnEpisodeBegin() {
        //sheepPigEnvironment.SpawnTraining();
        sheepPigEnvironment.MoveToNextAnimal();
    }

    public void TakeAction() {
        RequestDecision();
    }

    public override void OnActionReceived(ActionBuffers actions) {
        // Action: 0 = Sheep; 1 = Pig;
        bool selectSheep = actions.DiscreteActions[0] == 0;

        if (sheepPigEnvironment.TrySelectAnimal(selectSheep)) {
            AddReward(+1f);
            Debug.Log("Correct!");
        } else {
            AddReward(-1f);
            Debug.Log("Wrong!");
        }

        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        // KeyDown = Sheep; KeyUp = Pig;
        discreteActions[0] = Input.GetKey(KeyCode.T) ? 0 : 1;
    }

}
