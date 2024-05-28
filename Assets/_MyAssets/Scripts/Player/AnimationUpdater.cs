using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUpdater : MonoBehaviour
{
    private Animator _anim;
    private PlayerStateManager _playerState;

    private void Awake()
    {
        _playerState = PlayerStateManager.Instance;
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        _anim.SetBool(PlayerAnimationStates.DEAD, _playerState.CheckPlayerState(EPlayerState.Dead));
        _anim.SetBool(PlayerAnimationStates.IDLE, _playerState.CheckPlayerState(EPlayerState.Idle));
        _anim.SetBool(PlayerAnimationStates.WALK, _playerState.CheckPlayerState(EPlayerState.Walk));
        _anim.SetBool(PlayerAnimationStates.RUN, _playerState.CheckPlayerState(EPlayerState.Run));
        _anim.SetBool(PlayerAnimationStates.CROUCH, _playerState.CheckPlayerState(EPlayerState.Crouch));
        _anim.SetBool(PlayerAnimationStates.OVERSTEP, _playerState.CheckPlayerState(EPlayerState.Overstep));
        _anim.SetBool(PlayerAnimationStates.ITEM_READY, _playerState.CheckPlayerState(EPlayerState.ItemReady));
        _anim.SetBool(PlayerAnimationStates.ITEM_HOLD, _playerState.CheckPlayerState(EPlayerState.ItemHold));
        _anim.SetBool(PlayerAnimationStates.ITEM_THROW, _playerState.CheckPlayerState(EPlayerState.ItemThrow));
    }
}
