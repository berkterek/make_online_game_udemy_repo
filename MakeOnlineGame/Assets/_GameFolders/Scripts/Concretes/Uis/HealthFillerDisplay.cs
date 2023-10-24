using MakeOnlineGame.Combats;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace MakeOnlineGame.Uis
{
    public class HealthFillerDisplay : NetworkBehaviour
    {
        [SerializeField] Health _health;
        [SerializeField] Image _healthFiller;

        public override void OnNetworkSpawn()
        {
            if (!IsClient) return;
            
            _health.CurrentHealth.OnValueChanged += HandleOnCurrentValueChanged;
            HandleOnCurrentValueChanged(0, _health.MaxHealth);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient) return;
            
            _health.CurrentHealth.OnValueChanged -= HandleOnCurrentValueChanged;
        }
        
        void HandleOnCurrentValueChanged(int previousValue, int newValue)
        {
            _healthFiller.fillAmount = (float)newValue / _health.MaxHealth;
        }
    }    
}

