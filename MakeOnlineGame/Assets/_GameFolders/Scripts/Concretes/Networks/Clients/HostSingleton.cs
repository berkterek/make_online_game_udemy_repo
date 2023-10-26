using UnityEngine;

namespace MakeOnlineGame.Networks.Clients
{
    public class HostSingleton : MonoBehaviour
    {
        HostManager _hostManager;
        
        public static HostSingleton Instance { get; private set; }

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

        public void CreateClient()
        {
            _hostManager = new HostManager();
        }
    }
}