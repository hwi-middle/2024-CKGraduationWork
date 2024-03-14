using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScissorPlayer : PlayerMove
{
    [SerializeField] private PlayerData _myData;
    
    private GameObject _scissor;

    private IEnumerator _returnIdleWait;
    
    public bool IsWireAction { get; set; }
    private IEnumerator _wireActionWait;
    private Vector3 _wireTargetPosition;
    private float _wireRange;

    private static readonly Vector3 IDLE_POS = new(0.0f, 0.0f, -1.0f);
    private static readonly Quaternion IDLE_ROT = Quaternion.identity;
    
    private bool IsCrouch { get; set; }

    private void Start()
    {
        Debug.Assert(_myData.scissor != null, "_myData.scissor != null");
        _scissor = Instantiate(_myData.scissor, transform);
        _scissor.transform.localPosition = IDLE_POS;
        _scissor.transform.localRotation = IDLE_ROT;
    }
    
    protected override void Update()
    {
        if (IsWireAction)
        {
            return;
        }

        base.Update();
        ActivateIdleTimer();
    }

    // 맞은 위치가 방어 가능한 각도에 유효한지 판단
    public bool CheckAttackHitState(Vector3 hitPosition)
    {
        float guardAvailableDegree = Mathf.Deg2Rad * _myData.guardAvailableDegree;
        return Vector3.Dot(transform.position, hitPosition) > guardAvailableDegree;
    }

    public void OnNormalAttackButtonClick(InputAction.CallbackContext ctx)
    {
        if (ctx.started && PlayerManager.Instance.CurrentState == EPlayerState.Idle)
        {
            ScissorManager.Instance.ActivateAttackReady();
            return;
        }

        if (!ctx.started || PlayerManager.Instance.CurrentState == EPlayerState.Guard)
        {
            return;
        }

        ScissorManager.Instance.ActivateNormalAttack();
        PlayerManager.Instance.ChangePlayerState(EPlayerState.Attacking);
    }

    public void OnGuardAndParryButtonClick(InputAction.CallbackContext ctx)
    {
        if (PlayerManager.Instance.CurrentState == EPlayerState.Attacking)
        {
            return;
        }

        if (ctx.canceled)
        {
            ScissorManager.Instance.StopGuardCoroutine();
            ScissorManager.Instance.ActivateAttackReady();
            return;
        }

        PlayerManager.Instance.ChangePlayerState(EPlayerState.Guard);
        ScissorManager.Instance.ActivateGuard();
    }

    public void OnPositionMoveButtonClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }

        ScissorManager.Instance.ActivatePositionMove();
    }

    public void OnWireButtonClick(InputAction.CallbackContext ctx)
    {
        if (IsWireAction || !ctx.started)
        {
            return;
        }

        if (!ScissorManager.Instance.CanHangWire)
        {
            return;
        }

        ScissorManager.Instance.RotatePlayer();
        ApplyWireAction(ScissorManager.Instance.HitPosition, ScissorManager.Instance.WireRange);
    }

    private void ApplyWireAction(Vector3 wirePosition, float range)
    {
        const float Y_ADDITIVE_VALUE = 2.0f;
        IsWireAction = true;
        wirePosition.y += Y_ADDITIVE_VALUE;
        _wireTargetPosition = wirePosition;
        _wireRange = range;
        
        MovePlayerWireAction();
    }

    private void MovePlayerWireAction()
    {
        if (_wireActionWait != null)
        {
            return;
        }

        LineDraw.Instance.TurnOnLine();
        _wireActionWait = WireActionRoutine();
        ScissorManager.Instance.ActivateHangWire();
        StartCoroutine(_wireActionWait);
    }

    private IEnumerator WireActionRoutine()
    {
        const float TOLERANCE = 0.1f;
        
        while ((transform.position - _wireTargetPosition).magnitude >= TOLERANCE)
        {
            transform.position = Vector3.Lerp(transform.position, _wireTargetPosition, _myData.wireMoveTime * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        LineDraw.Instance.TurnOffLine();
        IsWireAction = false;
        _wireActionWait = null;
    }

    private void ActivateIdleTimer()
    {
        if (_returnIdleWait != null)
        {
            return;
        }

        _returnIdleWait = ReturnIdleRoutine();
        StartCoroutine(_returnIdleWait);
    }

    private IEnumerator ReturnIdleRoutine()
    {
        const float WAIT_TIME = 10.0f;
        yield return new WaitForSeconds(WAIT_TIME);
        ScissorManager.Instance.ReturnToIdle();
        PlayerManager.Instance.ChangePlayerState(EPlayerState.Idle);
    }

    public void OnRunButtonClick(InputAction.CallbackContext ctx)
    {
        // Move Direction == Vector3.zero 일 경우 return
    }

    public void OnCrouchButtonClick(InputAction.CallbackContext ctx)
    {
        // Move Direction == Vector3.zero 일 경우 return
        if (!ctx.started)
        {
            if (ctx.canceled)
            {
                IsCrouch = false;
            }

            return;
        }

        IsCrouch = true;
    }

    public void OnInteractionButtonClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        
        Debug.Log("Interaction Button Click");
    }
    
}