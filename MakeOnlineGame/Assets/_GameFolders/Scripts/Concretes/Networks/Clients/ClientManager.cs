using System.Text;
using Cysharp.Threading.Tasks;
using MakeOnlineGame.Controllers;
using MakeOnlineGame.Networks.Shares;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Clients
{
    public class ClientManager : System.IDisposable
    {
        JoinAllocation _allocation;
        NetworkClient _networkClient;
        
        public async UniTask<bool> InitializeAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            
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

            UserData userData = new UserData()
            {
                UserName = PlayerPrefs.GetString(NameSelectController.PLAYER_NAME_KEY, "Missing Name"),
                UserID = AuthenticationService.Instance.PlayerId
            };

            var payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            
            NetworkManager.Singleton.StartClient();
        }

        public void Dispose()
        {
            _networkClient?.Dispose();
        }
    }
}