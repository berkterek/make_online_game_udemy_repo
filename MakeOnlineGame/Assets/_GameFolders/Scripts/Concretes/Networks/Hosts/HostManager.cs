using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using MakeOnlineGame.Controllers;
using MakeOnlineGame.Networks.Servers;
using MakeOnlineGame.Networks.Shares;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Hosts
{
    public class HostManager
    {
        const int MAX_CONNECTION = 20;

        NetworkServer _networkServer;
        Allocation _allocation;
        string _joinCode;
        string _lobbyId;

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

            //unityTransport.SetRelayServerData(new RelayServerData(_allocation, "udp"));
            unityTransport.SetRelayServerData(new RelayServerData(_allocation, "dtls"));

            try
            {
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions();
                createLobbyOptions.IsPrivate = false;
                createLobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                    { "JoinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member, value: _joinCode) }
                };

                string playerName = PlayerPrefs.GetString(NameSelectController.PLAYER_NAME_KEY, "Unknown");
                var lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}' Lobby", MAX_CONNECTION,
                    createLobbyOptions);
                _lobbyId = lobby.Id;

                HearthBeatLobbyAsync(15);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return;
            }

            _networkServer = new NetworkServer(NetworkManager.Singleton);
            
            UserData userData = new UserData()
            {
                UserName = PlayerPrefs.GetString(NameSelectController.PLAYER_NAME_KEY, "Missing Name")
            };

            var payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        private async UniTask HearthBeatLobbyAsync(float waitSeconds)
        {
            while (true)
            {
                await Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
                await UniTask.Delay(System.TimeSpan.FromSeconds(waitSeconds), false, PlayerLoopTiming.Update);
            }
        }
    }
}