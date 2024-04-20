using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUpdater : MonoBehaviour
{
    private Dictionary<string, AnimationClip> _clips = new();
    
    private Animator _anim;
    private PlayerMove _playerMove;

    private void Awake()
    {
        _playerMove = PlayerMove.Instance;
        _anim = GetComponent<Animator>();
        
        // Animation Clip을 꺼내 쓰기 위한 Dictionary 저장
        foreach (AnimationClip clip in _anim.runtimeAnimatorController.animationClips)
        {
            string[] split = clip.name.Split('@');
            _clips.Add(split[1], clip);
        }
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
        _anim.SetBool(PlayerAnimationStates.WIREACTION, _playerMove.CheckPlayerState(EPlayerState.WireAction));
    }
}
