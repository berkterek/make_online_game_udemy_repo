using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class CoinSpawnerController : NetworkBehaviour
    {
        [SerializeField] RespawningCoinController _coinPrefab;
        [SerializeField] int _maxCoins = 50;
        [SerializeField] int _coinValue = 10;
        [SerializeField] Vector2 _xSpawnRange;
        [SerializeField] Vector2 _ySpawnRange;
        [SerializeField] LayerMask _layerMask;

        float _radius;
        readonly Collider2D[] _coinBuffer = new Collider2D[1];

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            _radius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

            for (int i = 0; i < _maxCoins; i++)
            {
                SpawnCoin();
            }
        }

        private async void SpawnCoin()
        {
            Vector2 randomPosition = await GetSpawnPoint();
            var coinInstance =Instantiate(_coinPrefab, randomPosition, Quaternion.identity);
            coinInstance.SetValue(_coinValue);
            coinInstance.GetComponent<NetworkObject>().Spawn();

            coinInstance.OnCollected += HandleOnCoinCollected;
        }

        void HandleOnCoinCollected(RespawningCoinController coin)
        {
            coin.transform.position = GetSpawnPoint().GetAwaiter().GetResult();
            coin.Reset();
        }

        private async UniTask<Vector2> GetSpawnPoint()
        {
            float x = 0f;
            float y = 0f;

            while (true)
            {
                x = Random.Range(_xSpawnRange.x, _xSpawnRange.y);
                y = Random.Range(_ySpawnRange.x, _ySpawnRange.y);
                Vector2 spawnPoint = new Vector2(x, y);

                int numberColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, _radius, _coinBuffer, _layerMask);

                if (numberColliders == 0) return spawnPoint;
                
                await UniTask.Yield();
            }
        }
    }    
}

