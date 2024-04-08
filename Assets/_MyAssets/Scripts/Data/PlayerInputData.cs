using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Player Input", menuName = "Scriptable Object Asset/Player Input")]
public class PlayerInputData : ScriptableObject, IA_Player.IPlayerActionActions
{
    private IA_Player _input;
    
    public Action<Vector2> moveEvent;
    public Action runEvent;
    public Action runQuitEvent;
    public Action jumpEvent;
    public Action wireEvent;
    public Action assassinateEvent;
    public Action crouchEvent;
    public Action pauseEvent;
    public Action aimingEvent;
    public Action aimingCancelEvent;
    public Action shootEvent;
    public Action hideEvent;
    public Action hideCancelEvent;

    private void OnEnable()
    {
        if (_input == null)
        {
            _input = new IA_Player();
            _input.PlayerAction.SetCallbacks(this);
        }

        _input.PlayerAction.Enable();
    }

    private void OnDisable()
    {
        _input?.PlayerAction.Disable();
    }

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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        jumpEvent?.Invoke();
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

    public void OnHide(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        hideEvent?.Invoke();
    }
}
