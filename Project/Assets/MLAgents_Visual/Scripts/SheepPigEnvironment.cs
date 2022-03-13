using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepPigEnvironment : MonoBehaviour {

    [SerializeField] private SheepPigAgent sheepPigAgent;
    [SerializeField] private Transform pfPig;
    [SerializeField] private Transform pfSheep;
    [SerializeField] private Transform pigPenTransform;
    [SerializeField] private Transform sheepPenTransform;

    private SheepPigMover agentSheepPigMover;
    private List<Transform> pigTransformList;
    private List<Transform> sheepTransformList;
    private List<Transform> animalTransformList;

    private bool lastTrainingIsSheep;
    private Transform lastTrainingTransform;

    private void Awake() {
        pigTransformList = new List<Transform>();
        sheepTransformList = new List<Transform>();
        animalTransformList = new List<Transform>();

        agentSheepPigMover = sheepPigAgent.GetComponent<SheepPigMover>();
        agentSheepPigMover.OnReachedTargetPosition += AgentSheepPigMover_OnReachedTargetPosition;

        SpawnInitialAnimals();
        //FunctionPeriodic.Create(() => sheepPigAgent.TakeAction(), 1f); // Used for Heuristic Testing
    }

    private void AgentSheepPigMover_OnReachedTargetPosition(object sender, System.EventArgs e) {
        sheepPigAgent.TakeAction();
    }

    private void SpawnInitialAnimals() {
        for (int i = 0; i < 20; i++) {
            SpawnPig();
            SpawnSheep();
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            MoveToNextAnimal();
        }
    }

    //private void FixedUpdate() { sheepPigAgent.TakeAction(); } // Used for Training

    public void MoveToNextAnimal() {
        if (animalTransformList.Count == 0) return; // No more animals!

        Transform animalTransform = animalTransformList[Random.Range(0, animalTransformList.Count)];
        animalTransformList.Remove(animalTransform);

        if (sheepTransformList.Contains(animalTransform)) {
            // Next Animal is a Sheep
            Debug.Log("Next Animal is a Sheep");
            lastTrainingIsSheep = true;
        } else {
            // Next Animal is a Pig
            Debug.Log("Next Animal is a Pig");
            lastTrainingIsSheep = false;
        }

        agentSheepPigMover.SetTargetPosition(animalTransform.position + (GetRandomDir() * 1.5f));
        agentSheepPigMover.SetLookAtPosition(animalTransform.position);

        lastTrainingTransform = animalTransform;
    }

    private Vector3 GetRandomDir() {
        return new Vector3(Random.Range(-1f, +1f), 0, Random.Range(-1f, +1f)).normalized;
    }

    private Vector3 GetSpawnPosition() {
        Vector3 spawnPosition;
        int safety = 0;
        int safetyMax = 1000;

        do {
            spawnPosition = new Vector3(Random.Range(-10f, +10f), 0, Random.Range(-13f, 13f));
            safety++;
        } while (Physics.OverlapBox(spawnPosition, new Vector3(2f, .5f, 2f)).Length != 0 && safety < safetyMax);

        if (safety >= safetyMax) {
            Debug.Log("SAFETY!");
        }

        return spawnPosition;
    }

    private Vector3 GetPigPenPosition() {
        return pigPenTransform.position + new Vector3(Random.Range(-8f, +8f), 0, Random.Range(-10f, +10f));
    }

    private Vector3 GetSheepPenPosition() {
        return sheepPenTransform.position + new Vector3(Random.Range(-8f, +8f), 0, Random.Range(-10f, +10f));
    }

    private void SendAnimalToPen(Transform animalTransform, bool sendToSheepPen) {
        Vector3 penPosition = sendToSheepPen ? GetSheepPenPosition() : GetPigPenPosition();
        animalTransform.GetComponent<SheepPigMover>().SetTargetPosition(penPosition);
        animalTransform.GetComponent<SheepPigMover>().SetLookAtPosition(penPosition + GetRandomDir());
    }

    private void SpawnPig() {
        Vector3 spawnPosition = GetSpawnPosition();
        Transform pigTransform = Instantiate(pfPig, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360f), 0));

        pigTransformList.Add(pigTransform);
        animalTransformList.Add(pigTransform);
    }

    private void SpawnSheep() {
        Vector3 spawnPosition = GetSpawnPosition();
        Transform sheepTransform = Instantiate(pfSheep, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360f), 0));

        sheepTransformList.Add(sheepTransform);
        animalTransformList.Add(sheepTransform);
    }

    public void SpawnTraining() {
        if (lastTrainingTransform != null) {
            Destroy(lastTrainingTransform.gameObject);
        }

        lastTrainingIsSheep = Random.Range(0, 100) < 50;
        Vector3 spawnPosition = sheepPigAgent.transform.position + (sheepPigAgent.transform.forward * 1.5f);

        Transform prefab = lastTrainingIsSheep ? pfSheep : pfPig;
        lastTrainingTransform = Instantiate(prefab, spawnPosition, sheepPigAgent.transform.rotation);
    }

    public bool IsLastTrainingSheep() {
        return lastTrainingIsSheep;
    }

    public bool TrySelectAnimal(bool isSheep) {
        Debug.Log((isSheep ? "Sheep" : "Pig") + " :: " + (lastTrainingIsSheep ? "Sheep" : "Pig"));

        SendAnimalToPen(lastTrainingTransform, isSheep);

        if (isSheep == lastTrainingIsSheep) {
            // Correct!
            return true;
        } else {
            // Wrong!
            return false;
        }
    }

}
