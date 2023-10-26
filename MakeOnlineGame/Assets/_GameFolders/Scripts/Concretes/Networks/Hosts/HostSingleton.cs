using UnityEngine;

namespace MakeOnlineGame.Networks.Hosts
{
    public class HostSingleton : MonoBehaviour
    {
        public HostManager HostManager { get; private set; }
        
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

        public void CreateHost()
        {
            HostManager = new HostManager();
        }
    }
}