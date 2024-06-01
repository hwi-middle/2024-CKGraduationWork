using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationStates : MonoBehaviour
{
    public static readonly int IDLE = Animator.StringToHash("IsIdle");
    public static readonly int WALK = Animator.StringToHash("IsWalk");
    public static readonly int RUN = Animator.StringToHash("IsRun");
    public static readonly int JUMP = Animator.StringToHash("IsJump");
    public static readonly int CROUCH = Animator.StringToHash("IsCrouch");
    public static readonly int SLIDING = Animator.StringToHash("IsSliding");
    public static readonly int DEAD = Animator.StringToHash("IsDead");
    public static readonly int OVERSTEP = Animator.StringToHash("IsOverstep");
    public static readonly int ITEM_READY = Animator.StringToHash("IsItemReady");
    public static readonly int ITEM_HOLD = Animator.StringToHash("IsItemHold");
    public static readonly int ITEM_THROW = Animator.StringToHash("IsItemThrow");
    public static readonly int ASSASSINATE = Animator.StringToHash("IsAssassinate");
}
