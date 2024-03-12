using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMoveAssassination : PlayerMove
{
    [SerializeField] private float _assassinationDuration;
    [SerializeField] private Transform _assassinationTarget;

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Assassinate();
        }
    }

    private void Assassinate()
    {
        PerformJump();
        StartCoroutine(AssassinateRoutine());
    }

    private IEnumerator AssassinateRoutine()
    {
        while (YVelocity > 0f)
        {
            yield return null;
        }
        
        float t = 0f;
        Vector3 initialPos = transform.position;

        while (t <= _assassinationDuration)
        {
            float alpha = t / _assassinationDuration;
            transform.position = Vector3.Lerp(initialPos, _assassinationTarget.position, alpha * alpha * alpha);

            yield return null;
            t += Time.deltaTime;
        }
    }
}