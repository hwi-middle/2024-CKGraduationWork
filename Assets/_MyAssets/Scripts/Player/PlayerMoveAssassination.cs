using System;
using System.Collections;
using TMPro;
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

    [SerializeField] private TMP_Text _noteTextForDebug;
    private bool _isAssassinating = false;

    private Transform _assassinationTarget;
    
    protected override void Update()
    {
        base.Update();

        if (!_isAssassinating)
        {
            _assassinationTarget = GetAimingEnemy();
        }
        
        // TODO: NIS 적용
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Assassinate();
        }
    }

    private Transform GetAimingEnemy()
    {
        Camera mainCamera = Camera.main;
        Debug.Assert(mainCamera != null);
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        int layerMask = LayerMask.GetMask("AssassinationTarget");
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 50f, Color.green, 0f, false);
        
        if (!Physics.Raycast(ray, out RaycastHit hit, 50f, layerMask))
        {
            _noteTextForDebug.text = "Note: Press Q to assassinate\nCurrent Target: None";
            return null;
        }
        
        _noteTextForDebug.text = $"Note: Press Q to assassinate\nCurrent Target: {hit.transform.name}";
        return hit.transform;
    }

    private void Assassinate()
    {
        Debug.Assert(_assassinationTarget != null);

        EAssassinationType assassinationType;
        if (transform.position.y - _assassinationTarget.position.y >= _assassinationData.jumpAssassinationHeightThreshold)
        {
            assassinationType = EAssassinationType.Jump;
        }
        else
        {
            assassinationType = EAssassinationType.Ground;
        }
        
        StartCoroutine(AssassinateRoutine(assassinationType));
    }
    
    private IEnumerator AssassinateRoutine(EAssassinationType assassinationType)
    {
        _isAssassinating = true;
        
        float assassinationDuration = 0f;
        switch (assassinationType)
        {
            case EAssassinationType.Ground:
                assassinationDuration = _assassinationData.groundAssassinationDuration;
                break;
            case EAssassinationType.Jump:
                assassinationDuration = _assassinationData.jumpAssassinationDuration;
                PerformJump();
        
                while (YVelocity > 0f)
                {
                    yield return null;
                }
                break;
            default:
                Debug.Assert(false);
                break;
        }
        
        float t = 0f;
        Vector3 initialPos = transform.position;
        
        while (t <= assassinationDuration)
        {
            float alpha = t / assassinationDuration;
            transform.position = Vector3.Lerp(initialPos, _assassinationTarget.position, alpha * alpha * alpha);

            yield return null;
            t += Time.deltaTime;
        }

        // TODO: 실제 적에게 데미지를 입혀야함

        _isAssassinating = false;
    }
}
