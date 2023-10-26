using Cysharp.Threading.Tasks;
using MakeOnlineGame.Inputs;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] InputReaderSO _inpuInputReader;
        [SerializeField] Transform _bodyTank;
        [SerializeField] Transform _turret;
        [SerializeField] Rigidbody2D _rigidbody;
        [SerializeField] float _movementSpeed = 4f;
        [SerializeField] float _turningRate = 30f;

        Vector2 _previousInput;
        Camera _mainCamera;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                this.gameObject.layer = 7;
                _bodyTank.gameObject.layer = 7;
                return;
            }

            FindMainCameraAsync();
            _inpuInputReader.OnMovement += HandleOnMovement;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            _inpuInputReader.OnMovement -= HandleOnMovement;
        }

        void Update()
        {
            if (!IsOwner) return;

            float zRotation = -_turningRate * Time.deltaTime * _previousInput.x;

            _bodyTank.Rotate(0f, 0f, zRotation);
        }

        void FixedUpdate()
        {
            if (!IsOwner) return;

            _rigidbody.velocity = _previousInput.y * _movementSpeed * (Vector2)_bodyTank.up;
        }

        void LateUpdate()
        {
            if (!IsOwner) return;

            if (_mainCamera == null) return;
            
            Vector2 worldLookPosition = _mainCamera.ScreenToWorldPoint(_inpuInputReader.LookPosition);

            Vector2 turretPosition = _turret.position;
            Vector2 turretLookPosition = new Vector2
            (
                worldLookPosition.x - turretPosition.x,
                worldLookPosition.y - turretPosition.y
            );

            _turret.up = turretLookPosition;
        }

        void HandleOnMovement(Vector2 value)
        {
            _previousInput = value;
        }

        private async void FindMainCameraAsync()
        {
            while (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                await UniTask.Yield();
            }
        }
    }
}