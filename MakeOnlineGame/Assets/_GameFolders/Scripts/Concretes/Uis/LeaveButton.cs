using MakeOnlineGame.Abstracts.Uis;
using MakeOnlineGame.Networks.Clients;
using MakeOnlineGame.Networks.Hosts;
using Unity.Netcode;

namespace MakeOnlineGame.Uis
{
    public class LeaveButton : BaseButton
    {
        protected override void HandleOnButtonClicked()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                HostSingleton.Instance.HostManager.Shutdown();
            }

            ClientSingleton.Instance.ClientManager.Disconnect();
        }
    }    
}