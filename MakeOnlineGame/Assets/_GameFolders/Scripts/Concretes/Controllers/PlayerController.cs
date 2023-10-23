using MakeOnlineGame.Inputs;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] InputReaderSO _inpuInputReader;
        [SerializeField] Transform _bodyTank;
        [SerializeField] Rigidbody2D _rigidbody;
        [SerializeField] float _movementSpeed = 4f;
        [SerializeField] float _turningRate = 30f;

        Vector2 _previousInput;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            _inpuInputReader.OnMovement += HandleOnMovement;
            _inpuInputReader.OnButtonClick += HandleOnButtonClicked;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            _inpuInputReader.OnMovement -= HandleOnMovement;
            _inpuInputReader.OnButtonClick -= HandleOnButtonClicked;
        }

        void Update()
        {
            if (!IsOwner) return;
            
            float zRotation = -_turningRate * Time.deltaTime * _previousInput.x;
            
            _bodyTank.Rotate(0f,0f,zRotation);
        }

        void FixedUpdate()
        {
            if (!IsOwner) return;

            _rigidbody.velocity = _previousInput.y * _movementSpeed * (Vector2)_bodyTank.up;
        }

        void HandleOnMovement(Vector2 value)
        {
            _previousInput = value;
        }
        
        void HandleOnButtonClicked(bool value)
        {
        }
    }
}