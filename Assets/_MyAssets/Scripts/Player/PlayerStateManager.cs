using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum EPlayerState
{
    None = 0,
    Idle = 1 << 1,
    Walk = 1 << 2,
    Run = 1 << 3,
    Crouch = 1 << 4,
    Hide = 1 << 5,
    Peek = 1 << 6,
    Alive = 1 << 7,
    Dead = 1 << 8,
    Overstep = 1 << 9,
}

public class PlayerStateManager : Singleton<PlayerStateManager>
{
    public int CurrentState { get; private set; } = (int)EPlayerState.Idle | (int)EPlayerState.Alive;

    public bool CheckPlayerState(EPlayerState state)
    {
        return (CurrentState & (int)state) != 0;
    }

    public void SetInitState()
    {
        CurrentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;
    }

    public void SetDeadState()
    {
        CurrentState = (int)EPlayerState.Dead;
    }

    public void AddPlayerState(EPlayerState state)
    {
        CurrentState |= (int)state;
    }

    public void RemovePlayerState(EPlayerState state)
    {
        CurrentState &= ~(int)state;
    }
}
