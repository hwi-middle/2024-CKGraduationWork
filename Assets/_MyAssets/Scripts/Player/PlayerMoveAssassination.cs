using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMoveAssassination : PlayerMove
{
    private enum EAssassinationType
    {
        Ground,
        Jump,
    }

    [SerializeField] private PlayerAssassinationData _assassinationData;
    private float _jumpAssassinationDuration;
    private float _groundAssassinationDuration;
    private float _jumpAssassinationHeightThreshold;

    [SerializeField] private Transform _assassinationTarget;

    protected override void Awake()
    {
        base.Awake();

        _jumpAssassinationDuration = _assassinationData.jumpAssassinationDuration;
        _groundAssassinationDuration = _assassinationData.groundAssassinationDuration;
        _jumpAssassinationHeightThreshold = _assassinationData.jumpAssassinationHeightThreshold;
    }

    protected override void Update()
    {
        base.Update();

        Camera mainCamera = Camera.main;
        Debug.Assert(mainCamera != null);
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        int layerMask = LayerMask.GetMask("AssassinationTarget");
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 50f, Color.green, 0f, false);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, layerMask))
        {
            Debug.Log(hit.transform.name);
        }
        
        // TODO: NIS 적용
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Assassinate();
        }
    }

    private void Assassinate()
    {
        // if(transform.position.y)

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

        while (t <= _jumpAssassinationDuration)
        {
            float alpha = t / _jumpAssassinationDuration;
            transform.position = Vector3.Lerp(initialPos, _assassinationTarget.position, alpha * alpha * alpha);

            yield return null;
            t += Time.deltaTime;
        }

        // TODO: 실제 적에게 데미지를 입혀야함
    }
}
