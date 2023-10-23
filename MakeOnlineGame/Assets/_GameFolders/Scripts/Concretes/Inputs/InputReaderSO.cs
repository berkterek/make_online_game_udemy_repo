using UnityEngine;
using UnityEngine.InputSystem;
using MakeOnlineGame.Abstracts.Inputs;
using static MakeOnlineGame.Inputs.GameInputActions;

namespace MakeOnlineGame.Inputs
{
    [CreateAssetMenu(menuName = "Terek Gaming/Inputs/New Input",fileName = "New Input")]
    public class InputReaderSO : ScriptableObject, IPlayerActions,IInputReader
    {
        GameInputActions _input;

        public event System.Action<Vector2> OnMovement;
        public event System.Action<bool> OnButtonClick;
        public Vector2 LookPosition { get; private set; }

        void OnEnable()
        {
            if (_input == null)
            {
                _input = new GameInputActions();
                _input.Player.SetCallbacks(this);
                
                _input.Enable();    
            }
        }

        void OnDisable()
        {
            if(_input != null) _input.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            OnMovement?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookPosition = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if(context.performed) OnButtonClick?.Invoke(true);
            else if(context.canceled) OnButtonClick?.Invoke(false);
        }
    }
}

