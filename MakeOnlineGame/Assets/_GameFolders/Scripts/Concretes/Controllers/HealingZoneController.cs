using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace MakeOnlineGame.Controllers
{
    public class HealingZoneController : NetworkBehaviour
    {
        [SerializeField] Image _healPowerBarImage;
        [SerializeField] int _maxHealPower = 30;
        [SerializeField] float _healCooldown = 60f;
        [SerializeField] float _healTickRate = 1f;
        [SerializeField] int _coinsPerTick = 10;
        [SerializeField] int _healthPerTick = 10;

        List<TankPlayerController> _playersInZone;
        NetworkVariable<int> _healPower = new NetworkVariable<int>();

        float _currentCooldown;
        float _tickTimer;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _playersInZone = new List<TankPlayerController>();
                _healPower.Value = _maxHealPower;
                _currentCooldown = _healCooldown;
            }

            if (IsClient)
            {
                _healPower.OnValueChanged += HandleOnValueChanged;
                HandleOnValueChanged(0,_healPower.Value);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient) _healPower.OnValueChanged -= HandleOnValueChanged;
        }

        void Update()
        {
            if (!IsServer) return;

            if (_currentCooldown > 0f)
            {
                _currentCooldown -= Time.deltaTime;

                if (_currentCooldown <= 0f)
                {
                    _healPower.Value = _maxHealPower;
                }
                else
                {
                    return;
                }
            }

            _tickTimer += Time.deltaTime;
            if (_tickTimer >= 1f / _healthPerTick)
            {
                foreach (var player in _playersInZone)
                {
                    if (_healPower.Value == 0) break;
                    
                    if(player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue;
                    
                    if(player.CoinCollectHandler.CoinTotal.Value < _coinsPerTick) continue;

                    player.CoinCollectHandler.SpendCoins(_coinsPerTick);
                    player.Health.RestoreHealth(_healthPerTick);

                    _healPower.Value -= 1;

                    if (_healPower.Value == 0)
                    {
                        _currentCooldown = _healCooldown;
                    }
                }

                _tickTimer = _tickTimer % (1 / _healCooldown);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(_playersInZone == null) return;

            if (!other.attachedRigidbody.TryGetComponent(out TankPlayerController tankPlayerController)) return;

            if (_playersInZone.Contains(tankPlayerController)) return;
            
            _playersInZone.Add(tankPlayerController);
            Debug.Log("Player Enter");
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if(_playersInZone == null) return;
            
            if (!other.attachedRigidbody.TryGetComponent(out TankPlayerController tankPlayerController)) return;

            if (!_playersInZone.Contains(tankPlayerController)) return;
            
            _playersInZone.Remove(tankPlayerController);
            Debug.Log("Player Exit");
        }
        
        void HandleOnValueChanged(int previousValue, int newValue)
        {
            _healPowerBarImage.fillAmount = (float)newValue / _maxHealPower;
        }
    }    
}

