using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public abstract class BaseCoinController : NetworkBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] int _coinValue = 1;
        [SerializeField] protected bool _isAlreadyCollected;

        public virtual int Collect()
        {
            return _coinValue;
        }

        public void SetValue(int value)
        {
            _coinValue = value;
        }

        public void ShowDisplay(bool value)
        {
            _spriteRenderer.enabled = value;
        }
    }
}