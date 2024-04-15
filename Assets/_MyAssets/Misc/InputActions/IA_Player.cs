//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/_MyAssets/Misc/InputActions/IA_Player.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @IA_Player: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @IA_Player()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""IA_Player"",
    ""maps"": [
        {
            ""name"": ""PlayerAction"",
            ""id"": ""cdf64783-f9b4-4f3e-b543-ffb6c01cd012"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""7eff2f9d-daf2-46af-b36b-655cfab95cbe"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""c9fafe89-4a67-4523-b9da-2c84f891dbb7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""30834a8a-5b3d-466c-a882-ff8aeb9c1e81"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Wire"",
                    ""type"": ""Button"",
                    ""id"": ""a8213568-675a-4e6f-9131-535bee700080"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Assassinate"",
                    ""type"": ""Button"",
                    ""id"": ""c806ddfd-84d4-4e85-a89f-bef7e0450b60"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""9c0a6b18-ce2b-45b3-b0ff-aa956f6fb92b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""e890f7ba-64cc-410b-9e09-488a0d2492e4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aiming"",
                    ""type"": ""Button"",
                    ""id"": ""9cae9f6c-8c2b-494f-bb9e-7852d26c88fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""859511e1-e30c-4dd8-b77f-57aff93c7f99"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""54119653-70fc-4afb-a567-2aa127137507"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""74f989c8-da51-4009-bacb-2dcbaf5ab422"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""4e7511f7-a5d2-43ac-b6f6-be1fbbe59619"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a6f81a4f-a7f7-4e04-9f72-ba738f110cec"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8ac7bc98-ea76-41b8-9779-5943821fe4cb"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""88e87f66-71c8-4672-9ae7-797aa295fda1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2ac94f8d-0721-47bc-aa86-809307221d9a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6bb6b0dd-0548-42cb-bbbd-ed203beb2a2a"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Wire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c61f962-9c27-473a-87d5-c3e9006e355a"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Assassinate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""636f800d-97fe-4c29-8365-be420d420570"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d77d8df-4536-4b32-b3e4-6c9db733c620"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""584aabb4-5a3a-4af4-8ebd-e0e647189054"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76e3e850-9aa6-43a4-b704-d5d8148948bb"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Aiming"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""51f72d70-ee4b-45f3-a958-098a436c5d5e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8f41f95-3436-4473-9ab9-6822a3ca4d67"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""HideAction"",
            ""id"": ""2621908e-f51a-4096-9046-1185fe0701d1"",
            ""actions"": [
                {
                    ""name"": ""Peek"",
                    ""type"": ""Button"",
                    ""id"": ""a6666a57-2a62-4ba0-92f6-80f163b5d1f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Exit"",
                    ""type"": ""Value"",
                    ""id"": ""b45a26a2-7f21-48b0-8864-0396895e0660"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseAxis"",
                    ""type"": ""Value"",
                    ""id"": ""8ba2651e-c9f9-49c6-a115-35e3afb74ad6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5c112874-54f1-4610-a165-940767c902ed"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Peek"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Move"",
                    ""id"": ""e6943423-3ddf-4641-b261-249eae3d6a99"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c4d0284d-7a9c-4f1b-a9ef-29a5bb079884"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""441d1f8b-40b6-4781-95ba-41e452ae4efa"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ab5443cd-8732-4535-8cd7-3f83ef1ea397"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5a3db6ef-0e2c-4c8e-98a4-3249d0d7665d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""83069f03-721d-40e1-9090-3ccf8018a0ea"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""MouseAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""MOBILE"",
            ""bindingGroup"": ""MOBILE"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerAction
        m_PlayerAction = asset.FindActionMap("PlayerAction", throwIfNotFound: true);
        m_PlayerAction_Move = m_PlayerAction.FindAction("Move", throwIfNotFound: true);
        m_PlayerAction_Run = m_PlayerAction.FindAction("Run", throwIfNotFound: true);
        m_PlayerAction_Jump = m_PlayerAction.FindAction("Jump", throwIfNotFound: true);
        m_PlayerAction_Wire = m_PlayerAction.FindAction("Wire", throwIfNotFound: true);
        m_PlayerAction_Assassinate = m_PlayerAction.FindAction("Assassinate", throwIfNotFound: true);
        m_PlayerAction_Crouch = m_PlayerAction.FindAction("Crouch", throwIfNotFound: true);
        m_PlayerAction_Pause = m_PlayerAction.FindAction("Pause", throwIfNotFound: true);
        m_PlayerAction_Aiming = m_PlayerAction.FindAction("Aiming", throwIfNotFound: true);
        m_PlayerAction_Shoot = m_PlayerAction.FindAction("Shoot", throwIfNotFound: true);
        m_PlayerAction_Interaction = m_PlayerAction.FindAction("Interaction", throwIfNotFound: true);
        // HideAction
        m_HideAction = asset.FindActionMap("HideAction", throwIfNotFound: true);
        m_HideAction_Peek = m_HideAction.FindAction("Peek", throwIfNotFound: true);
        m_HideAction_Exit = m_HideAction.FindAction("Exit", throwIfNotFound: true);
        m_HideAction_MouseAxis = m_HideAction.FindAction("MouseAxis", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerAction
    private readonly InputActionMap m_PlayerAction;
    private List<IPlayerActionActions> m_PlayerActionActionsCallbackInterfaces = new List<IPlayerActionActions>();
    private readonly InputAction m_PlayerAction_Move;
    private readonly InputAction m_PlayerAction_Run;
    private readonly InputAction m_PlayerAction_Jump;
    private readonly InputAction m_PlayerAction_Wire;
    private readonly InputAction m_PlayerAction_Assassinate;
    private readonly InputAction m_PlayerAction_Crouch;
    private readonly InputAction m_PlayerAction_Pause;
    private readonly InputAction m_PlayerAction_Aiming;
    private readonly InputAction m_PlayerAction_Shoot;
    private readonly InputAction m_PlayerAction_Interaction;
    public struct PlayerActionActions
    {
        private @IA_Player m_Wrapper;
        public PlayerActionActions(@IA_Player wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerAction_Move;
        public InputAction @Run => m_Wrapper.m_PlayerAction_Run;
        public InputAction @Jump => m_Wrapper.m_PlayerAction_Jump;
        public InputAction @Wire => m_Wrapper.m_PlayerAction_Wire;
        public InputAction @Assassinate => m_Wrapper.m_PlayerAction_Assassinate;
        public InputAction @Crouch => m_Wrapper.m_PlayerAction_Crouch;
        public InputAction @Pause => m_Wrapper.m_PlayerAction_Pause;
        public InputAction @Aiming => m_Wrapper.m_PlayerAction_Aiming;
        public InputAction @Shoot => m_Wrapper.m_PlayerAction_Shoot;
        public InputAction @Interaction => m_Wrapper.m_PlayerAction_Interaction;
        public InputActionMap Get() { return m_Wrapper.m_PlayerAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActionActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Wire.started += instance.OnWire;
            @Wire.performed += instance.OnWire;
            @Wire.canceled += instance.OnWire;
            @Assassinate.started += instance.OnAssassinate;
            @Assassinate.performed += instance.OnAssassinate;
            @Assassinate.canceled += instance.OnAssassinate;
            @Crouch.started += instance.OnCrouch;
            @Crouch.performed += instance.OnCrouch;
            @Crouch.canceled += instance.OnCrouch;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @Aiming.started += instance.OnAiming;
            @Aiming.performed += instance.OnAiming;
            @Aiming.canceled += instance.OnAiming;
            @Shoot.started += instance.OnShoot;
            @Shoot.performed += instance.OnShoot;
            @Shoot.canceled += instance.OnShoot;
            @Interaction.started += instance.OnInteraction;
            @Interaction.performed += instance.OnInteraction;
            @Interaction.canceled += instance.OnInteraction;
        }

        private void UnregisterCallbacks(IPlayerActionActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Wire.started -= instance.OnWire;
            @Wire.performed -= instance.OnWire;
            @Wire.canceled -= instance.OnWire;
            @Assassinate.started -= instance.OnAssassinate;
            @Assassinate.performed -= instance.OnAssassinate;
            @Assassinate.canceled -= instance.OnAssassinate;
            @Crouch.started -= instance.OnCrouch;
            @Crouch.performed -= instance.OnCrouch;
            @Crouch.canceled -= instance.OnCrouch;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @Aiming.started -= instance.OnAiming;
            @Aiming.performed -= instance.OnAiming;
            @Aiming.canceled -= instance.OnAiming;
            @Shoot.started -= instance.OnShoot;
            @Shoot.performed -= instance.OnShoot;
            @Shoot.canceled -= instance.OnShoot;
            @Interaction.started -= instance.OnInteraction;
            @Interaction.performed -= instance.OnInteraction;
            @Interaction.canceled -= instance.OnInteraction;
        }

        public void RemoveCallbacks(IPlayerActionActions instance)
        {
            if (m_Wrapper.m_PlayerActionActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActionActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActionActions @PlayerAction => new PlayerActionActions(this);

    // HideAction
    private readonly InputActionMap m_HideAction;
    private List<IHideActionActions> m_HideActionActionsCallbackInterfaces = new List<IHideActionActions>();
    private readonly InputAction m_HideAction_Peek;
    private readonly InputAction m_HideAction_Exit;
    private readonly InputAction m_HideAction_MouseAxis;
    public struct HideActionActions
    {
        private @IA_Player m_Wrapper;
        public HideActionActions(@IA_Player wrapper) { m_Wrapper = wrapper; }
        public InputAction @Peek => m_Wrapper.m_HideAction_Peek;
        public InputAction @Exit => m_Wrapper.m_HideAction_Exit;
        public InputAction @MouseAxis => m_Wrapper.m_HideAction_MouseAxis;
        public InputActionMap Get() { return m_Wrapper.m_HideAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(HideActionActions set) { return set.Get(); }
        public void AddCallbacks(IHideActionActions instance)
        {
            if (instance == null || m_Wrapper.m_HideActionActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_HideActionActionsCallbackInterfaces.Add(instance);
            @Peek.started += instance.OnPeek;
            @Peek.performed += instance.OnPeek;
            @Peek.canceled += instance.OnPeek;
            @Exit.started += instance.OnExit;
            @Exit.performed += instance.OnExit;
            @Exit.canceled += instance.OnExit;
            @MouseAxis.started += instance.OnMouseAxis;
            @MouseAxis.performed += instance.OnMouseAxis;
            @MouseAxis.canceled += instance.OnMouseAxis;
        }

        private void UnregisterCallbacks(IHideActionActions instance)
        {
            @Peek.started -= instance.OnPeek;
            @Peek.performed -= instance.OnPeek;
            @Peek.canceled -= instance.OnPeek;
            @Exit.started -= instance.OnExit;
            @Exit.performed -= instance.OnExit;
            @Exit.canceled -= instance.OnExit;
            @MouseAxis.started -= instance.OnMouseAxis;
            @MouseAxis.performed -= instance.OnMouseAxis;
            @MouseAxis.canceled -= instance.OnMouseAxis;
        }

        public void RemoveCallbacks(IHideActionActions instance)
        {
            if (m_Wrapper.m_HideActionActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IHideActionActions instance)
        {
            foreach (var item in m_Wrapper.m_HideActionActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_HideActionActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public HideActionActions @HideAction => new HideActionActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    private int m_MOBILESchemeIndex = -1;
    public InputControlScheme MOBILEScheme
    {
        get
        {
            if (m_MOBILESchemeIndex == -1) m_MOBILESchemeIndex = asset.FindControlSchemeIndex("MOBILE");
            return asset.controlSchemes[m_MOBILESchemeIndex];
        }
    }
    public interface IPlayerActionActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnWire(InputAction.CallbackContext context);
        void OnAssassinate(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnAiming(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnInteraction(InputAction.CallbackContext context);
    }
    public interface IHideActionActions
    {
        void OnPeek(InputAction.CallbackContext context);
        void OnExit(InputAction.CallbackContext context);
        void OnMouseAxis(InputAction.CallbackContext context);
    }
}
