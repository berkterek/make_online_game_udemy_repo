using MakeOnlineGame.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Handlers
{
    public class CoinCollectHandler : NetworkBehaviour
    {
        public NetworkVariable<int> _coinTotal = new NetworkVariable<int>();

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BaseCoinController coin))
            {
                int coinValue = coin.Collect();

                if (!IsServer) return;

                _coinTotal.Value += coinValue;
            }
        }
    }
}

