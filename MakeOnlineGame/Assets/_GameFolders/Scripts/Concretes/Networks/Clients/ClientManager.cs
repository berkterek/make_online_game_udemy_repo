using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Clients
{
    public class ClientManager 
    {
        JoinAllocation _allocation;
        
        public async UniTask<bool> InitializeAsync()
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Connected to Unity Service");

            var state = await AuthenticationWrapper.DoAuthenticate();

            return state == AuthenticateState.Auhenticated;
        }

        public async void GoToMenu()
        {
            await SceneManager.LoadSceneAsync("Menu");
        }

        public async UniTask StartClientAsync(string joinCode)
        {
            try
            {
                _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }

            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(new RelayServerData(_allocation, "dtls"));

            NetworkManager.Singleton.StartClient();
        }
    }
}