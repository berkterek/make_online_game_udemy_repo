using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MakeOnlineGame.Networks.Clients
{
    public class ClientSingleton : MonoBehaviour
    {
        public ClientManager ClientManager { get; private set; }
        public static ClientSingleton Instance { get; private set; }

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

        public async UniTask<bool> CreateClientAsync()
        {
            ClientManager = new ClientManager();

            bool isAuthResult = await ClientManager.InitializeAsync();
            return isAuthResult;
        }
    }
}