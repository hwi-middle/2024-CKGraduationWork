using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Player Input", menuName = "Scriptable Object Asset/Player Input")]
public class PlayerInputData : ScriptableObject, IA_Player.IPlayerActionActions, IA_Player.IHideActionActions, IA_Player.ISettingActionActions, IA_Player.ISettingDetailActionActions
{
    public enum EInputMap
    {
        PlayerAction,
        HideAction,
        SettingAction,
        SettingDetailAction
    }
    
    private static IA_Player _input;
    private static List<InputActionMap> _inputActionMaps = new();
    
    // Player Action Map
    public Action<Vector2> moveEvent;
    public Action runEvent;
    public Action runQuitEvent;
    public Action wireEvent;
    public Action assassinateEvent;
    public Action crouchEvent;
    public Action aimingEvent;
    public Action aimingCancelEvent;
    public Action shootEvent;
    public Action clairvoyanceEvent;
    
    // Player Action Map 중 Gamepad Action
    public Action<Vector2, bool> gamepadAxisEvent;
    private Vector2 _gamepadAxis;
        
    // Hide Action Map
    public Action interactionEvent;
    public Action hideExitEvent;
    public Action peekEvent;
    public Action peekExitEvent;
    public Action<float> mouseAxisEvent;
    public Action<Vector2, bool> peekGamepadAxisEvent;
    
    // Setting Action Map
    public Action<float> tabMoveEvent;
    public Action settingSubmitEvent;
    
    // Setting Detail Action Map
    public Action backEvent;
    
    // 모든 Action Map에 적용
    public Action pauseEvent;
    
    private void OnEnable()
    {
        if (_input == null)
        {
            _input = new IA_Player();
            _input.PlayerAction.SetCallbacks(this);
            _input.HideAction.SetCallbacks(this);
            _input.SettingAction.SetCallbacks(this);
            _input.SettingDetailAction.SetCallbacks(this);
            
            _inputActionMaps.Add(_input.PlayerAction);
            _inputActionMaps.Add(_input.HideAction);
            _inputActionMaps.Add(_input.SettingAction);
            _inputActionMaps.Add(_input.SettingDetailAction);
        }

        _input.PlayerAction.Enable();
    }

    private void OnDisable()
    {
        foreach(InputActionMap map in _inputActionMaps)
        {
            map.Disable();
        }
    }

    // 모든 Action Map에 적용
    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        pauseEvent?.Invoke();
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
        
        interactionEvent?.Invoke();
    }

    public void OnClairvoyance(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        clairvoyanceEvent?.Invoke();
    }

    public void OnGamepadAxis(InputAction.CallbackContext context)
    {
        Vector2 axis = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            axis = Vector2.zero;
            gamepadAxisEvent?.Invoke(axis, false);
            return;
        }

        gamepadAxisEvent?.Invoke(axis, true);
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

    public void OnPeekGamepadAxis(InputAction.CallbackContext context)
    {
        Vector2 axis = context.ReadValue<Vector2>();

        if (context.performed)
        {
            return;
        }

        if (context.canceled)
        {
            axis = Vector2.zero;
            peekGamepadAxisEvent?.Invoke(axis, false);
            return;
        }

        peekGamepadAxisEvent?.Invoke(axis, true);
    }
    
    // Setting Action Map
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        settingSubmitEvent?.Invoke();
    }
    
    public void OnTabMove(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        tabMoveEvent?.Invoke(context.ReadValue<float>());
    }
    
    // Setting Detail Action Map
    
    #region Input System UI Input Module 영역

    public void OnDetailSubmit(InputAction.CallbackContext context)
    {
    }
    
    public void OnDetailMove(InputAction.CallbackContext context)
    {
    }
    
    #endregion 
    
    public void OnBack(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        
        backEvent?.Invoke();
    }

    public static void ChangeInputMap(EInputMap map)
    {
        foreach (InputActionMap inputActionMap in _inputActionMaps)
        {
            if (inputActionMap.name == map.ToString())
            {
                inputActionMap.Enable();
                continue;
            }

            inputActionMap.Disable();
        }
    }
}