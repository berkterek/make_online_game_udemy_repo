using MakeOnlineGame.Networks.Shares;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Networks.Servers
{
    public class NetworkServer
    {
        readonly NetworkManager _networkManager;
        
        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.ConnectionApprovalCallback += HandleOnConnectionApprovalCallback;
        }

        ~NetworkServer()
        {
            _networkManager.ConnectionApprovalCallback -= HandleOnConnectionApprovalCallback;
        }

        void HandleOnConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var userData = JsonUtility.FromJson<UserData>(payload);

            Debug.Log(userData.UserName);

            response.Approved = true;
            response.CreatePlayerObject = true;
        }
    }    
}

