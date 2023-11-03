using System.Collections.Generic;
using MakeOnlineGame.Controllers;
using MakeOnlineGame.Networks.Shares;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Networks.Servers
{
    public class NetworkServer : System.IDisposable
    {
        readonly NetworkManager _networkManager;
        readonly Dictionary<ulong, string> _clientIdToAuth;
        readonly Dictionary<string, UserData> _authIdToUserData;

        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _clientIdToAuth = new Dictionary<ulong, string>();
            _authIdToUserData = new Dictionary<string, UserData>();

            _networkManager.ConnectionApprovalCallback += HandleOnConnectionApprovalCallback;
            _networkManager.OnServerStarted += HandleOnServerStarted;
        }

        void HandleOnConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var userData = JsonUtility.FromJson<UserData>(payload);

            _clientIdToAuth[request.ClientNetworkId] = userData.UserID;
            _authIdToUserData[userData.UserID] = userData;

            response.Approved = true;
            response.Position = SpawnPointController.GetRandomSpawnPosition();
            response.Rotation = Quaternion.identity;
            response.CreatePlayerObject = true;
        }
        
        void HandleOnServerStarted()
        {
            _networkManager.OnClientDisconnectCallback += HandleOnClientOnDisconnect;
        }

        void HandleOnClientOnDisconnect(ulong clientNetworkId)
        {
            if (_clientIdToAuth.TryGetValue(clientNetworkId, out string authId))
            {
                _clientIdToAuth.Remove(clientNetworkId);
                _authIdToUserData.Remove(authId);
            }
        }

        void ReleaseUnmanagedResources()
        {
            _networkManager.ConnectionApprovalCallback -= HandleOnConnectionApprovalCallback;
            _networkManager.OnServerStarted -= HandleOnServerStarted;
            _networkManager.OnClientDisconnectCallback -= HandleOnClientOnDisconnect;
        }

        public void Dispose()
        {
            if (_networkManager != null)
            {
                ReleaseUnmanagedResources();                
            }
            
            if(_networkManager.IsListening) _networkManager.Shutdown();

            System.GC.SuppressFinalize(this);
        }

        public UserData GetUserDataByClientId(ulong clientId)
        {
            if (_clientIdToAuth.TryGetValue(clientId, out string auth))
            {
                if (_authIdToUserData.TryGetValue(auth, out UserData userData))
                {
                    return userData;
                }
            }

            return default;
        }
    }    
}

