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
        [SerializeField] Texture2D _crosshair;
        [SerializeField] Health _health;
        [SerializeField] CoinCollectHandler _coinCollectHandler;
        [SerializeField] CinemachineVirtualCamera _cinemachineVirtual;
        [SerializeField] SpriteRenderer _minimapSpriteRenderer;

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
                _minimapSpriteRenderer.color = Color.blue;
                Cursor.SetCursor(_crosshair, new Vector2(_crosshair.width / 2f, _crosshair.height / 2f), CursorMode.Auto);
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

