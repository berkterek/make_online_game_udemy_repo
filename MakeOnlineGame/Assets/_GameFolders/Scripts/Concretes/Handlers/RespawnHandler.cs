using Cysharp.Threading.Tasks;
using MakeOnlineGame.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Handlers
{
    public class RespawnHandler : NetworkBehaviour
    {
        [SerializeField] TankPlayerController _playerPrefab;
        [SerializeField] float _keepCoinPercentage;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            TankPlayerController[] players = FindObjectsByType<TankPlayerController>(FindObjectsSortMode.None);

            foreach (var player in players)
            {
                HandleOnPlayerSpawned(player);
            }

            TankPlayerController.OnPlayerSpawned += HandleOnPlayerSpawned;
            TankPlayerController.OnPlayerDespawned += HandleOnPlayerDespawned;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            TankPlayerController.OnPlayerSpawned -= HandleOnPlayerSpawned;
            TankPlayerController.OnPlayerDespawned -= HandleOnPlayerDespawned;
        }
        
        void HandleOnPlayerSpawned(TankPlayerController value)
        {
            value.Health.OnDead += (health) =>
            {
                HandleDead(value);
            };
        }

        void HandleOnPlayerDespawned(TankPlayerController value)
        {
            // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
            value.Health.OnDead -= (health) =>
            {
                HandleDead(value);
            };
        }

        private void HandleDead(TankPlayerController value)
        {
            int lastCoinValue = (int)(value.CoinCollectHandler.CoinTotal.Value * (_keepCoinPercentage / 100f));
            Destroy(value.gameObject);

            RespawnPlayerAsync(value.OwnerClientId, lastCoinValue);
        }

        private async UniTask RespawnPlayerAsync(ulong clientId, int keptCoins)
        {
            await UniTask.Yield();

            var playerInstance = Instantiate(_playerPrefab, SpawnPointController.GetRandomSpawnPosition(), Quaternion.identity);

            playerInstance.NetworkObject.SpawnAsPlayerObject(clientId);
            
            playerInstance.CoinCollectHandler.CoinTotal.Value += keptCoins;
        }
    }
}