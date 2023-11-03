using Cysharp.Threading.Tasks;
using MakeOnlineGame.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Handlers
{
    public class RespawnHandler : NetworkBehaviour
    {
        [SerializeField] NetworkObject _playerPrefab;

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
            Destroy(value.gameObject);

            RespawnPlayerAsync(value.OwnerClientId);
        }

        private async UniTask RespawnPlayerAsync(ulong clientId)
        {
            await UniTask.Yield();

            var playerInstance = Instantiate(_playerPrefab, SpawnPointController.GetRandomSpawnPosition(), Quaternion.identity);
            
            playerInstance.SpawnAsPlayerObject(clientId);
        }
    }
}