using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerceptionPhaseUiHandler : MonoBehaviour
{
    [SerializeField] private EnemyBase _enemyBase;
    private TMP_Text _perceptionPhaseText;


    private void Awake()
    {
        _perceptionPhaseText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        _perceptionPhaseText.text = _enemyBase.GetCurrentPerceptionPhase().ToString();
    }
}
