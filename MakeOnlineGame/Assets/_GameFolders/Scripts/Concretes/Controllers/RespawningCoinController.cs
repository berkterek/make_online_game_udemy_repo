using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class RespawningCoinController : BaseCoinController
    {
        [SerializeField] Transform _transform;
        
        Vector3 _previousPosition;

        public event System.Action<RespawningCoinController> OnCollected;

        void OnValidate()
        {
            if (_transform == null) _transform = GetComponent<Transform>();
        }

        void Update()
        {
            if (_previousPosition != _transform.position)
            {
                ShowDisplay(true);
            }

            _previousPosition = _transform.position;
        }
        
        public override int Collect()
        {
            if (!IsServer)
            {
                ShowDisplay(false);
                return 0;
            }

            if (_isAlreadyCollected) return 0;

            _isAlreadyCollected = true;
            OnCollected?.Invoke(this);

            return base.Collect();
        }

        public void Reset()
        {
            _isAlreadyCollected = false;
        }
    }
}