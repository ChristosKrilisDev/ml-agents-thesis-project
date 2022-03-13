using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimation : MonoBehaviour {

    [SerializeField] private SheepPigMover sheepPigMover;
    [SerializeField] private Animator animalAnimator;

    private void Start() {
        PlayAnimIdle();

        sheepPigMover.OnStateChanged += SheepPigMover_OnStateChanged;
    }

    private void SheepPigMover_OnStateChanged(object sender, SheepPigMover.OnStateChangedEventArgs e) {
        switch (e.state) {
            case SheepPigMover.State.Idle:
            case SheepPigMover.State.Looking:
                PlayAnimIdle();
                break;
            case SheepPigMover.State.Moving:
                PlayAnimWalk();
                break;
        }
    }

    private void PlayAnimIdle() {
        //animalAnimator.SetFloat("Speed_f", 0f);
    }

    private void PlayAnimWalk() {
        //animalAnimator.SetFloat("Speed_f", 1.5f);
    }

}
