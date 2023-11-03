using Cinemachine;
using MakeOnlineGame.Networks.Hosts;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class TankPlayerController : NetworkBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _cinemachineVirtual;

        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                var userData = HostSingleton.Instance.HostManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                PlayerName.Value = userData.UserName;
            }
            
            if (IsOwner)
            {
                _cinemachineVirtual.Priority = 12;
            }
        }
    }    
}

