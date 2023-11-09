using MakeOnlineGame.Networks.Clients;
using MakeOnlineGame.Networks.Hosts;
using MakeOnlineGame.Networks.Servers;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

namespace MakeOnlineGame.Controllers
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] ClientSingleton _clientSingleton;
        [SerializeField] HostSingleton _hostSingleton;
        [SerializeField] ServerSingleton _serverSingleton;
        
        ReactiveProperty<bool> _isDedicatedServer = new ReactiveProperty<bool>(); 
        
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            _isDedicatedServer.Subscribe(async (isDedicatedServer) =>
            {
                if (isDedicatedServer)
                {
                    var serverSingleton = Instantiate(_serverSingleton);
                    await serverSingleton.CreateServerAsync();

                    await serverSingleton.ServerManager.StartGameServerAsync();
                }
                else
                {
                    var clientSingletonVariable = Instantiate(_clientSingleton);
                    bool isClientAuth = await clientSingletonVariable.CreateClientAsync();

                    var hostSingletonVariable = Instantiate(_hostSingleton);
                    hostSingletonVariable.CreateHost();

                    if (isClientAuth)
                    {
                        //GO to main menu
                        clientSingletonVariable.ClientManager.GoToMenu();
                    }
                }
            });
            
            _isDedicatedServer.Value = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }
    }    
}