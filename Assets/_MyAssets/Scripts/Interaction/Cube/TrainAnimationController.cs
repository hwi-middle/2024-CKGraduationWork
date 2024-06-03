using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainAnimationController : MonoBehaviour
{
    [SerializeField] private Animator[] _trainAnimators;
    [SerializeField] private Animator[] _barrierAnimators;
    [SerializeField] private GameObject[] _trainEffects;

    private void Start()
    {
        foreach (Animator trainAnimator in _trainAnimators)
        {
            trainAnimator.enabled = false;
        }

        foreach (Animator barrierAnimator in _barrierAnimators)
        {
            barrierAnimator.enabled = false;
        }
        
        foreach(GameObject effect in _trainEffects)
        {
            effect.SetActive(false);
        }
    }

    public void PlayTrainAnimation()
    {
        foreach (Animator trainAnimator in _trainAnimators)
        {
            trainAnimator.enabled = true;
        }
        
        foreach(Animator barrierAnimator in _barrierAnimators)
        {
            barrierAnimator.enabled = true;
        }

        foreach (GameObject effect in _trainEffects)    
        {
            effect.SetActive(true);
        }
    }
}
