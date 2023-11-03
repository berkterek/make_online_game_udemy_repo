using Cinemachine;
using MakeOnlineGame.Combats;
using MakeOnlineGame.Handlers;
using MakeOnlineGame.Networks.Hosts;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class TankPlayerController : NetworkBehaviour
    {
        [SerializeField] Health _health;
        [SerializeField] CoinCollectHandler _coinCollectHandler;
        [SerializeField] CinemachineVirtualCamera _cinemachineVirtual;

        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        public static event System.Action<TankPlayerController> OnPlayerSpawned;
        public static event System.Action<TankPlayerController> OnPlayerDespawned;

        public Health Health => _health;
        public CoinCollectHandler CoinCollectHandler => _coinCollectHandler;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                var userData = HostSingleton.Instance.HostManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                PlayerName.Value = userData.UserName;
                OnPlayerSpawned?.Invoke(this);
            }
            
            if (IsOwner)
            {
                _cinemachineVirtual.Priority = 12;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                OnPlayerDespawned?.Invoke(this);
            }
        }
    }    
}

