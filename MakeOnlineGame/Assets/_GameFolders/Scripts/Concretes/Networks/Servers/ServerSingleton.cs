using Cysharp.Threading.Tasks;
using MakeOnlineGame.Networks.Clients;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

namespace MakeOnlineGame.Networks.Servers
{
    public class ServerSingleton : MonoBehaviour
    {
        public ServerManager ServerManager { get; private set; }
        public static ServerSingleton Instance { get; private set; }

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void OnDestroy()
        {
            ServerManager.Dispose();
        }

        public async UniTask CreateServerAsync()
        {
            await UnityServices.InitializeAsync();

            ServerManager = new ServerManager(
                ApplicationData.IP(),
                ApplicationData.Port(),
                ApplicationData.QPort(),
                NetworkManager.Singleton);
        }
    }
}