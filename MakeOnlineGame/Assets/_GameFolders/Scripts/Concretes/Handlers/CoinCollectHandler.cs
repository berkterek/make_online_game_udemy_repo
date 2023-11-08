using Cysharp.Threading.Tasks;
using MakeOnlineGame.Combats;
using MakeOnlineGame.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Handlers
{
    public class CoinCollectHandler : NetworkBehaviour
    {
        [SerializeField] Health _health;
        [SerializeField] BountyCoinController _coinPrefab;
        [SerializeField] int _bountyCoinCount = 10;
        [SerializeField] float _bountyPercentage = 50f;
        [SerializeField] int _minBountyCoinValue = 5;
        [SerializeField] float _coinSpread = 3f;
        [SerializeField] LayerMask _layerMask;
        
        public NetworkVariable<int> CoinTotal = new NetworkVariable<int>();
        
        float _radius;
        readonly Collider2D[] _coinBuffer = new Collider2D[1];

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            _radius = _coinPrefab.GetComponent<CircleCollider2D>().radius;
            
            _health.OnDead += HandleOnPlayerDead;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            _health.OnDead -= HandleOnPlayerDead;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BaseCoinController coin))
            {
                int coinValue = coin.Collect();

                if (!IsServer) return;

                CoinTotal.Value += coinValue;
            }
        }
        
        void HandleOnPlayerDead(Health health)
        {
            int bountyValue = (int)(CoinTotal.Value * (_bountyPercentage / 100f));
            int bountyCoinValue = bountyValue / _bountyCoinCount;

            if (bountyCoinValue < _minBountyCoinValue) return;

            for (int i = 0; i < _bountyCoinCount; i++)
            {
                var coinInstance = Instantiate(_coinPrefab, GetSpawnPoint().GetAwaiter().GetResult(), Quaternion.identity);
                coinInstance.SetValue(bountyCoinValue);
                coinInstance.NetworkObject.Spawn();
            }
        }
        
        private async UniTask<Vector2> GetSpawnPoint()
        {
            while (true)
            {
                Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * _coinSpread;

                int numberColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, _radius, _coinBuffer, _layerMask);

                if (numberColliders == 0) return spawnPoint;
                
                await UniTask.Yield();
            }
        }
    }
}