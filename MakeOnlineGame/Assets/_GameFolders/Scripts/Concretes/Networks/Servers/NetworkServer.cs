using System.Collections.Generic;
using MakeOnlineGame.Networks.Shares;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Networks.Servers
{
    public class NetworkServer
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

        ~NetworkServer()
        {
            _networkManager.ConnectionApprovalCallback -= HandleOnConnectionApprovalCallback;
            _networkManager.OnServerStarted -= HandleOnServerStarted;
        }

        void HandleOnConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var userData = JsonUtility.FromJson<UserData>(payload);

            _clientIdToAuth[request.ClientNetworkId] = userData.UserID;
            _authIdToUserData[userData.UserID] = userData;

            response.Approved = true;
            response.CreatePlayerObject = true;
        }
        
        void HandleOnServerStarted()
        {
            _networkManager.OnClientDisconnectCallback += HandleOnClientOnDisconnect;
        }

        void HandleOnClientOnDisconnect(ulong clientNetworkId)
        {
            _networkManager.OnClientDisconnectCallback -= HandleOnClientOnDisconnect;

            if (_clientIdToAuth.TryGetValue(clientNetworkId, out string authId))
            {
                _clientIdToAuth.Remove(clientNetworkId);
                _authIdToUserData.Remove(authId);
            }
        }
    }    
}

