using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUpdateHelper : MonoBehaviour
{
    private Animator _anim;
    private PlayerMove _playerMove;

    private void Awake()
    {
        _playerMove = GetComponentInParent<PlayerMove>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        _anim.SetBool(AnimationStates.IDLE, _playerMove.CheckPlayerState(EPlayerState.Idle));
        _anim.SetBool(AnimationStates.WALK, _playerMove.CheckPlayerState(EPlayerState.Walk));
        _anim.SetBool(AnimationStates.RUN, _playerMove.CheckPlayerState(EPlayerState.Run));
        _anim.SetBool(AnimationStates.JUMP, _playerMove.CheckPlayerState(EPlayerState.Jump));
        _anim.SetBool(AnimationStates.CROUCH, _playerMove.CheckPlayerState(EPlayerState.Crouch));
        _anim.SetBool(AnimationStates.SLIDING, _playerMove.CheckPlayerState(EPlayerState.Sliding));
        _anim.SetBool(AnimationStates.WIREACTION, _playerMove.CheckPlayerState(EPlayerState.WireAction));
    }
}
