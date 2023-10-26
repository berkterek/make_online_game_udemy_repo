using MakeOnlineGame.Networks.Clients;
using MakeOnlineGame.Networks.Hosts;
using UnityEngine;
using UnityEngine.UI;

namespace MakeOnlineGame.Controllers
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] Button _hostButton;
        [SerializeField] Button _clientButton;

        void OnEnable()
        {
            _hostButton.onClick.AddListener(HandleOnHostButtonClicked);
            _clientButton.onClick.AddListener(HandleOnClientButtonClicked);
        }

        void OnDisable()
        {
            _hostButton.onClick.RemoveListener(HandleOnHostButtonClicked);
            _clientButton.onClick.RemoveListener(HandleOnClientButtonClicked);
        }
        
        async void HandleOnHostButtonClicked()
        {
            await HostSingleton.Instance.HostManager.StartHostAsync();
        }
        
        async void HandleOnClientButtonClicked()
        {
            // await ClientSingleton.Instance.ClientManager.
        }
    }    
}

