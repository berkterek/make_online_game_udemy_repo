using MakeOnlineGame.Abstracts.Inputs;
using UnityEngine;

namespace MakeOnlineGame.Inputs
{
    public class InputReader : IInputReader
    {
        readonly GameInputActions _input;

        public event System.Action<Vector2> OnMovement;
        public event System.Action<bool> OnButtonClick;
        public Vector2 LookPosition { get; }

        public InputReader()
        {
            _input = new GameInputActions();
            
            _input.Enable();
        }

        ~InputReader()
        {
            _input.Disable();
        }
    }
}