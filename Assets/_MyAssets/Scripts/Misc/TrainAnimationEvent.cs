using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrainAnimationEvent : MonoBehaviour
{
    [SerializeField] private GameObject _trainRailSparkEffect;
    [SerializeField] private GameObject _enemy;
    
    public void OnTrainAnimationEnd()
    {
        _trainRailSparkEffect.SetActive(false);
        _enemy.SetActive(true);
    }
}
