using MakeOnlineGame.Networks.Clients;
using MakeOnlineGame.Networks.Hosts;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

namespace MakeOnlineGame.Controllers
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] ClientSingleton _clientSingleton;
        [SerializeField] HostSingleton _hostSingleton;
        
        ReactiveProperty<bool> _isDedicatedServer = new ReactiveProperty<bool>(); 
        
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            _isDedicatedServer.Subscribe(async (isDedicatedServer) =>
            {
                if (isDedicatedServer)
                {
                
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