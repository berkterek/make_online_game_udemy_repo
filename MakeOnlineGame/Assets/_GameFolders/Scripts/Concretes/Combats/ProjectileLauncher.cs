using Cysharp.Threading.Tasks;
using MakeOnlineGame.Inputs;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Combats
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [SerializeField] InputReaderSO _inputReader;
        [SerializeField] Transform _projectileSpawnPoint;
        [SerializeField] GameObject _projectileServerPrefab;
        [SerializeField] GameObject _projectileClientPrefab;
        [SerializeField] GameObject _muzzleFlash;
        [SerializeField] float _projectileSpeed = 0f;
        [SerializeField] float _fireRate;
        [SerializeField] float _muzzleFlashDuration;

        bool _canFire;
        float _previousFireTime;
        float _muzzleFlashTimer;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            _inputReader.OnButtonClick += HandleOnButtonClicked;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            _inputReader.OnButtonClick -= HandleOnButtonClicked;
        }

        void Update()
        {
            if (!IsOwner) return;

            if (!_canFire) return;

            if (Time.time < (1 / _fireRate) + _previousFireTime) return;

            PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
            SpawnClientProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

            _previousFireTime = Time.time;
        }

        void SpawnClientProjectile(Vector3 position, Vector3 direction)
        {
            MuzzleFlashShowDisableAsync();
            var projectile = Instantiate(_projectileClientPrefab, position, Quaternion.identity);
            projectile.transform.up = direction;
            projectile.layer = IsOwner ? 8 : 9;

            if (projectile.TryGetComponent(out Rigidbody2D rigidbody2D))
            {
                rigidbody2D.velocity = _projectileSpeed * rigidbody2D.transform.up;
            }
        }

        private async void MuzzleFlashShowDisableAsync()
        {
            _muzzleFlash.SetActive(true);
            await UniTask.Delay(System.TimeSpan.FromSeconds(_muzzleFlashDuration));
            _muzzleFlash.SetActive(false);
        }

        [ServerRpc]
        void PrimaryFireServerRpc(Vector3 position, Vector3 direction)
        {
            var projectile = Instantiate(_projectileServerPrefab, position, Quaternion.identity);
            projectile.transform.up = direction;
            projectile.layer = IsOwner ? 8 : 9;
            if (projectile.TryGetComponent(out DealDamageOnContact dealDamageOnContact))
            {
                dealDamageOnContact.SetOwner(OwnerClientId);
            }
            
            if (projectile.TryGetComponent(out Rigidbody2D rigidbody2D))
            {
                rigidbody2D.velocity = _projectileSpeed * rigidbody2D.transform.up;
            }
            
            PrimaryFireClientRpc(position, direction);
        }

        [ClientRpc]
        void PrimaryFireClientRpc(Vector3 position, Vector3 direction)
        {
            if (IsOwner) return;

            SpawnClientProjectile(position, direction);
        }

        void HandleOnButtonClicked(bool value)
        {
            _canFire = value;
        }
    }
}