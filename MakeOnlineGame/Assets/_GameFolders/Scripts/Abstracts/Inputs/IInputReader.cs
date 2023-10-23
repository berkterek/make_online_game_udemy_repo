using UnityEngine;

namespace MakeOnlineGame.Abstracts.Inputs
{
    public interface IInputReader
    {
        event System.Action<Vector2> OnMovement;
        event System.Action<bool> OnButtonClick;
    }
}