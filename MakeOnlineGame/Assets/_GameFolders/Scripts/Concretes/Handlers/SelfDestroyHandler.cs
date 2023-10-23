using UnityEngine;

namespace MakeOnlineGame.Handlers
{
    public class SelfDestroyHandler : MonoBehaviour
    {
        [SerializeField] LifeTimeCounterHandler _lifeTimeCounterHandler;

        void OnEnable()
        {
            _lifeTimeCounterHandler.OnLifeTimeEnded += HandleOnSelfDestroy;
        }

        void OnDisable()
        {
            _lifeTimeCounterHandler.OnLifeTimeEnded -= HandleOnSelfDestroy;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            HandleOnSelfDestroy();
        }

        private void HandleOnSelfDestroy()
        {
            Destroy(this.gameObject);
        }
    }    
}