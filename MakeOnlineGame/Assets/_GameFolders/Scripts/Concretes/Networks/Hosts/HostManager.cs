using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Hosts
{
    public class HostManager
    {
        const int MAX_CONNECTION = 20;

        Allocation _allocation;
        string _joinCode;
        
        public async UniTask StartHostAsync()
        {
            try
            {
                _allocation = await Relay.Instance.CreateAllocationAsync(MAX_CONNECTION);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return;
            }

            try
            {
                _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                Debug.Log("Join Code => " + _joinCode);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return;
            }

            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            
            unityTransport.SetRelayServerData(new RelayServerData(_allocation, "udp"));

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
}