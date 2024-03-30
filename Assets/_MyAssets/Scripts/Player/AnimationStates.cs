using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStates : MonoBehaviour
{
    public static readonly int IDLE = Animator.StringToHash("IsIdle");
    public static readonly int WALK = Animator.StringToHash("IsWalk");
    public static readonly int RUN = Animator.StringToHash("IsRun");
    public static readonly int JUMP = Animator.StringToHash("IsJump");
    public static readonly int CROUCH = Animator.StringToHash("IsCrouch");
    public static readonly int SLIDING = Animator.StringToHash("IsSliding");
    public static readonly int WIREACTION = Animator.StringToHash("IsWireAction");
}
