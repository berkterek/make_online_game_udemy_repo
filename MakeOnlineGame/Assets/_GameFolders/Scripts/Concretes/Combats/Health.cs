using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Combats
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] int _maxHealth = 100;
        public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

        public int MaxHealth => _maxHealth;
        public bool IsDead => CurrentHealth.Value <= 0;
        public event System.Action<Health> OnDead;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            CurrentHealth.Value = _maxHealth;
        }

        public void TakeDamage(int damageValue)
        {
            ModifyHealth(-damageValue);
        }

        public void RestoreHealth(int healthValue)
        {
            ModifyHealth(healthValue);
        }

        private void ModifyHealth(int value)
        {
            if (IsDead) return;

            int result = CurrentHealth.Value + value;
            CurrentHealth.Value = Mathf.Clamp(result, 0, _maxHealth);

            if (IsDead)
            {
                OnDead?.Invoke(this);
            }
        }
    }
}