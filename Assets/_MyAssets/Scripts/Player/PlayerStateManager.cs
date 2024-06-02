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
    ItemReady = 1 << 10,
    ItemHold = 1 << 11,
    ItemThrow = 1 << 12,
    Assassinate = 1 << 13,
}

public class PlayerStateManager : Singleton<PlayerStateManager>
{
    private int _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;

    public bool CheckPlayerState(EPlayerState state)
    {
        return (_currentState & (int)state) != 0;
    }

    public void SetInitState()
    {
        _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;
    }

    public void SetDeadState()
    {
        _currentState = (int)EPlayerState.Dead;
    }

    public void AddPlayerState(EPlayerState state)
    {
        _currentState |= (int)state;
    }

    public void RemovePlayerState(EPlayerState state)
    {
        _currentState &= ~(int)state;
    }
}
