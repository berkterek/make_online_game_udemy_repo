using MakeOnlineGame.Inputs;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] InputReaderSO _inpuInputReader;

        void OnEnable()
        {
            _inpuInputReader.OnMovement += HandleOnMovement;
            _inpuInputReader.OnButtonClick += HandleOnButtonClicked;
        }

        void OnDisable()
        {
            _inpuInputReader.OnMovement -= HandleOnMovement;
            _inpuInputReader.OnButtonClick -= HandleOnButtonClicked;
        }
        
        void HandleOnMovement(Vector2 value)
        {
            Debug.Log(value);
        }
        
        void HandleOnButtonClicked(bool value)
        {
            Debug.Log(value);   
        }
    }
}