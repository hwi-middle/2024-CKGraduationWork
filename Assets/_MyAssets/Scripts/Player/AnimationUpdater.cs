using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUpdater : MonoBehaviour
{
    private Animator _anim;
    private PlayerMove _playerMove;

    private void Awake()
    {
        _playerMove = PlayerMove.Instance;
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        _anim.SetBool(PlayerAnimationStates.DEAD, _playerMove.CheckPlayerState(EPlayerState.Dead));
        _anim.SetBool(PlayerAnimationStates.IDLE, _playerMove.CheckPlayerState(EPlayerState.Idle));
        _anim.SetBool(PlayerAnimationStates.WALK, _playerMove.CheckPlayerState(EPlayerState.Walk));
        _anim.SetBool(PlayerAnimationStates.RUN, _playerMove.CheckPlayerState(EPlayerState.Run));
        _anim.SetBool(PlayerAnimationStates.CROUCH, _playerMove.CheckPlayerState(EPlayerState.Crouch));
        _anim.SetBool(PlayerAnimationStates.OVERSTEP, _playerMove.CheckPlayerState(EPlayerState.Overstep));
        _anim.SetBool(PlayerAnimationStates.ASSASSINATE, _playerMove.CheckPlayerState(EPlayerState.Assassinate));
    }

    public void OnAttackAnimationStart()
    {
        _playerMove.UpdateEnemyDeadState();
    }
}
