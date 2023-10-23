using UnityEngine;

namespace MakeOnlineGame.Handlers
{
    public class LifeTimeCounterHandler : MonoBehaviour
    {
        [SerializeField] float _maxLifeTime = 10f;

        float _currentTime = 0f;

        public event System.Action OnLifeTimeEnded;

        void Update()
        {
            _currentTime += Time.deltaTime;
            
            if(_currentTime > _maxLifeTime) OnLifeTimeEnded?.Invoke();
        }
    }    
}