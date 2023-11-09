using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Servers
{
    public class ServerManager : System.IDisposable
    {
        const string GAME_SCENE_NAME = "Game"; 
        
        readonly string _serverIP;
        readonly int _serverPort;
        readonly int _serverQPort;
        readonly NetworkServer _networkServer;
        readonly MultiplayAllocationService _multiplayAllocationService;
        
        public ServerManager(string serverIP, int serverPort, int serverQPort, NetworkManager networkManager)
        {
            _serverIP = serverIP;
            _serverPort = serverPort;
            _serverQPort = serverQPort;
            _networkServer = new NetworkServer(networkManager);
            _multiplayAllocationService = new MultiplayAllocationService();
        }
        
        public async UniTask StartGameServerAsync()
        {
            await _multiplayAllocationService.BeginServerCheck();

            if (!_networkServer.OpenConnection(_serverIP, _serverPort))
            {
                Debug.LogWarning("Network server did not start as expected!");
                return;
            }

            NetworkManager.Singleton.SceneManager.LoadScene(GAME_SCENE_NAME, LoadSceneMode.Single);
        }
        
        public void Dispose()
        {
            _multiplayAllocationService?.Dispose();
            _networkServer?.Dispose();
        }
    }
}