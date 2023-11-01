using System.Collections.Generic;
using MakeOnlineGame.Networks.Shares;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Clients
{
    public class NetworkClient
    {
        const string MENU_NAME = "Menu";
        readonly NetworkManager _networkManager;

        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.OnClientDisconnectCallback += HandleOnClientOnDisconnect;
        }

        ~NetworkClient()
        {
            _networkManager.OnClientDisconnectCallback -= HandleOnClientOnDisconnect;
        }

        void HandleOnClientOnDisconnect(ulong clientNetworkId)
        {
            if (clientNetworkId != 0 && clientNetworkId != _networkManager.LocalClient.ClientId) return;

            //if we are in game start menu scene
            if (SceneManager.GetActiveScene().name != MENU_NAME)
            {
                SceneManager.LoadScene(MENU_NAME);
            }

            //if we are in menu try connected as a client shutdown for try again
            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown();
            }
        }
    }
}