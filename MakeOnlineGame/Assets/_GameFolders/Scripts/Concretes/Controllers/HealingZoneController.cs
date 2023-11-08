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

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            _playersInZone = new List<TankPlayerController>();
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
    }    
}

