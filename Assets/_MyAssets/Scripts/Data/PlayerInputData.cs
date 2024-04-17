using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Player Input", menuName = "Scriptable Object Asset/Player Input")]
public class PlayerInputData : ScriptableObject, IA_Player.IPlayerActionActions, IA_Player.IHideActionActions
{
    public enum EInputMap
    {
        PlayerAction,
        HideAction
    }
    
    private static IA_Player _input;
    
    // Player Action
    public Action<Vector2> moveEvent;
    public Action runEvent;
    public Action runQuitEvent;
    public Action wireEvent;
    public Action assassinateEvent;
    public Action crouchEvent;
    public Action pauseEvent;
    public Action aimingEvent;
    public Action aimingCancelEvent;
    public Action shootEvent;
    public Action clairvoyanceEvent;
    
    // Hide Action
    public Action hideEvent;
    public Action hideExitEvent;
    public Action peekEvent;
    public Action peekExitEvent;
    public Action<float> mouseAxisEvent;

    private void OnEnable()
    {
        if (_input == null)
        {
            _input = new IA_Player();
            _input.PlayerAction.SetCallbacks(this);
            _input.HideAction.SetCallbacks(this);
        }

        _input.PlayerAction.Enable();
    }

    private void OnDisable()
    {
        _input?.PlayerAction.Disable();
        _input?.HideAction.Disable();
    }

    // Player Action Map
    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            if (context.canceled)
            {
                runQuitEvent?.Invoke();
            }
            return;
        }
        
        runEvent?.Invoke();
    }

    public void OnWire(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        wireEvent?.Invoke();
    }

    public void OnAssassinate(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        assassinateEvent?.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        crouchEvent?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        pauseEvent?.Invoke();
    }

    public void OnAiming(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            return;
        }

        if (context.canceled)
        {
            aimingCancelEvent?.Invoke();
            return;
        }
        
        aimingEvent?.Invoke();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        shootEvent?.Invoke();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        hideEvent?.Invoke();
    }

    public void OnClairvoyance(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        clairvoyanceEvent?.Invoke();
    }

    // Hide Action Map
    public void OnPeek(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            return;
        }

        if (context.canceled)
        {
            peekExitEvent?.Invoke();
            return;
        }
        
        peekEvent?.Invoke();
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        hideExitEvent?.Invoke();
    }

    public void OnMouseAxis(InputAction.CallbackContext context)
    {
        float xAxis = context.ReadValue<Vector2>().x;

        if (xAxis == 0)
        {
            return;
        }
        
        mouseAxisEvent?.Invoke(xAxis);
    }

    public static void ChangeInputMap(EInputMap map)
    {
        switch (map)
        {
            case EInputMap.PlayerAction:
                _input.HideAction.Disable();
                _input.PlayerAction.Enable();
                break;
            case EInputMap.HideAction:
                _input.PlayerAction.Disable();
                _input.HideAction.Enable();
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }
}