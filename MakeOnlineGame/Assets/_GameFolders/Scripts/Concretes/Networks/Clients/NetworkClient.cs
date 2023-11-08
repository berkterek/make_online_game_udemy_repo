using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Clients
{
    public class NetworkClient : System.IDisposable 
    {
        const string MENU_NAME = "Menu";
        readonly NetworkManager _networkManager;

        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.OnClientDisconnectCallback += HandleOnClientOnDisconnect;
        }
        
        void HandleOnClientOnDisconnect(ulong clientNetworkId)
        {
            if (clientNetworkId != 0 && clientNetworkId != _networkManager.LocalClient.ClientId) return;

            Disconnect();
        }

        void ReleaseUnmanagedResources()
        {
            _networkManager.OnClientDisconnectCallback -= HandleOnClientOnDisconnect;
        }

        public void Dispose()
        {
            if (_networkManager != null)
            {
                ReleaseUnmanagedResources();    
            }
            
            System.GC.SuppressFinalize(this);
        }

        public void Disconnect()
        {
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