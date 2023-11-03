using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class TankPlayerController : NetworkBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _cinemachineVirtual;
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _cinemachineVirtual.Priority = 12;
            }
        }
    }    
}

