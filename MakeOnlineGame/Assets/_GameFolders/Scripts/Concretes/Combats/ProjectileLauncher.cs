using System;
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
        [SerializeField] float _projectileSpeed = 0f;

        bool _canFire;

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

            PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
            SpawnClientProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        }

        void SpawnClientProjectile(Vector3 position, Vector3 direction)
        {
            var projectile = Instantiate(_projectileClientPrefab, position, Quaternion.identity);
            projectile.transform.up = direction;
        }

        [ServerRpc]
        void PrimaryFireServerRpc(Vector3 position, Vector3 direction)
        {
            var projectile = Instantiate(_projectileServerPrefab, position, Quaternion.identity);
            projectile.transform.up = direction;

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