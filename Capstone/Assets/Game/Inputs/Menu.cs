// GENERATED AUTOMATICALLY FROM 'Assets/Game/Inputs/Menu.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Menu : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Menu()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Menu"",
    ""maps"": [
        {
            ""name"": ""MenuActions"",
            ""id"": ""9b5c6f2e-ebd3-45e2-a0b6-3e68f3a38dd8"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""c4acf005-8d22-4506-8b0f-116712743b45"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""43fb1554-d575-4018-a2ff-d6fcb3529e76"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20f90925-37dd-497d-8fd3-ead1baf20995"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PauseMenuActions"",
            ""id"": ""216f7cc8-4cfc-418c-a9e5-1e8c9550e24f"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""63e56450-0785-4220-a887-3b3516a454a4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Resume"",
                    ""type"": ""Button"",
                    ""id"": ""1ffcc715-a59b-46dc-af08-23f4cfd4f96e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2dfcd264-4971-4721-bfc6-e2355f59cd1b"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b2c24b2d-7a54-4a57-9c54-59ad4c7ee03a"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e882aa92-2127-4300-8bad-7b851562b7d5"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Resume"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ecfdb55a-54a2-4f98-bf5c-846ab69b7f01"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Resume"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MenuActions
        m_MenuActions = asset.FindActionMap("MenuActions", throwIfNotFound: true);
        m_MenuActions_Pause = m_MenuActions.FindAction("Pause", throwIfNotFound: true);
        // PauseMenuActions
        m_PauseMenuActions = asset.FindActionMap("PauseMenuActions", throwIfNotFound: true);
        m_PauseMenuActions_Pause = m_PauseMenuActions.FindAction("Pause", throwIfNotFound: true);
        m_PauseMenuActions_Resume = m_PauseMenuActions.FindAction("Resume", throwIfNotFound: true);
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

    // MenuActions
    private readonly InputActionMap m_MenuActions;
    private IMenuActionsActions m_MenuActionsActionsCallbackInterface;
    private readonly InputAction m_MenuActions_Pause;
    public struct MenuActionsActions
    {
        private @Menu m_Wrapper;
        public MenuActionsActions(@Menu wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_MenuActions_Pause;
        public InputActionMap Get() { return m_Wrapper.m_MenuActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActionsActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActionsActions instance)
        {
            if (m_Wrapper.m_MenuActionsActionsCallbackInterface != null)
            {
                @Pause.started -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_MenuActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public MenuActionsActions @MenuActions => new MenuActionsActions(this);

    // PauseMenuActions
    private readonly InputActionMap m_PauseMenuActions;
    private IPauseMenuActionsActions m_PauseMenuActionsActionsCallbackInterface;
    private readonly InputAction m_PauseMenuActions_Pause;
    private readonly InputAction m_PauseMenuActions_Resume;
    public struct PauseMenuActionsActions
    {
        private @Menu m_Wrapper;
        public PauseMenuActionsActions(@Menu wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_PauseMenuActions_Pause;
        public InputAction @Resume => m_Wrapper.m_PauseMenuActions_Resume;
        public InputActionMap Get() { return m_Wrapper.m_PauseMenuActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseMenuActionsActions set) { return set.Get(); }
        public void SetCallbacks(IPauseMenuActionsActions instance)
        {
            if (m_Wrapper.m_PauseMenuActionsActionsCallbackInterface != null)
            {
                @Pause.started -= m_Wrapper.m_PauseMenuActionsActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PauseMenuActionsActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PauseMenuActionsActionsCallbackInterface.OnPause;
                @Resume.started -= m_Wrapper.m_PauseMenuActionsActionsCallbackInterface.OnResume;
                @Resume.performed -= m_Wrapper.m_PauseMenuActionsActionsCallbackInterface.OnResume;
                @Resume.canceled -= m_Wrapper.m_PauseMenuActionsActionsCallbackInterface.OnResume;
            }
            m_Wrapper.m_PauseMenuActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Resume.started += instance.OnResume;
                @Resume.performed += instance.OnResume;
                @Resume.canceled += instance.OnResume;
            }
        }
    }
    public PauseMenuActionsActions @PauseMenuActions => new PauseMenuActionsActions(this);
    public interface IMenuActionsActions
    {
        void OnPause(InputAction.CallbackContext context);
    }
    public interface IPauseMenuActionsActions
    {
        void OnPause(InputAction.CallbackContext context);
        void OnResume(InputAction.CallbackContext context);
    }
}
