using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeOnlineGame.Networks.Clients
{
    public class ClientManager 
    {
        public async UniTask<bool> InitializeAsync()
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Connected to Unity Service");

            var state = await AuthenticationWrapper.DoAuthenticate();

            return state == AuthenticateState.Auhenticated;
        }

        public async void GoToMenu()
        {
            await SceneManager.LoadSceneAsync(1);
        }
    }
}