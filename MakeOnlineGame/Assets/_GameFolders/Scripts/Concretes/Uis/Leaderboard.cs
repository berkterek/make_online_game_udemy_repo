using System.Collections.Generic;
using System.Linq;
using MakeOnlineGame.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Uis
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] Transform _leaderboardEntityHolder;
        [SerializeField] LeaderboardEntityDisplay _prefab;
        [SerializeField] int _entitiesToDisplay = 8;

        NetworkList<LeaderboardEntityState> _leaderboardEntityStates;
        List<LeaderboardEntityDisplay> _leaderboardEntityDisplays;

        void Awake()
        {
            _leaderboardEntityStates = new NetworkList<LeaderboardEntityState>();
            _leaderboardEntityDisplays = new List<LeaderboardEntityDisplay>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _leaderboardEntityStates.OnListChanged += HandleOnListChanged;

                foreach (var leaderboardEntityState in _leaderboardEntityStates)
                {
                    HandleOnListChanged(new NetworkListEvent<LeaderboardEntityState>()
                    {
                        Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                        Value = leaderboardEntityState
                    });
                }
            }

            if (IsServer)
            {
                TankPlayerController[] players = FindObjectsByType<TankPlayerController>(FindObjectsSortMode.None);

                foreach (var player in players)
                {
                    HandleOnPlayerSpawn(player);
                }

                TankPlayerController.OnPlayerSpawned += HandleOnPlayerSpawn;
                TankPlayerController.OnPlayerDespawned += HandleOnPlayerDespawn;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _leaderboardEntityStates.OnListChanged -= HandleOnListChanged;
            }

            if (IsServer)
            {
                TankPlayerController.OnPlayerSpawned -= HandleOnPlayerSpawn;
                TankPlayerController.OnPlayerDespawned -= HandleOnPlayerDespawn;
            }
        }

        void HandleOnListChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                    var addValue = changeEvent.Value;
                    if (_leaderboardEntityDisplays.All(x => x.ClientId != addValue.ClientId))
                    {
                        var leaderboardEntityDisplay = Instantiate(_prefab, _leaderboardEntityHolder);
                        leaderboardEntityDisplay.SetData(addValue.ClientId,addValue.PlayerName,addValue.Coin);
                        _leaderboardEntityDisplays.Add(leaderboardEntityDisplay);    
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                    var removeValue = changeEvent.Value;
                    var removeDisplay = _leaderboardEntityDisplays.FirstOrDefault(x => x.ClientId == removeValue.ClientId);
                    if (removeDisplay != null)
                    {
                        Destroy(removeDisplay.gameObject);
                        _leaderboardEntityDisplays.Remove(removeDisplay);
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                    var updatedValue = changeEvent.Value;
                    var updatedDisplay =
                        _leaderboardEntityDisplays.FirstOrDefault(x => x.ClientId == updatedValue.ClientId);
                    if (updatedDisplay != null)
                    {
                        updatedDisplay.UpdateCoin(updatedValue.Coin);
                    }
                    break;
            }
            
            _leaderboardEntityDisplays.Sort((first,second) => second.Coin.CompareTo(first.Coin));

            for (int i = 0; i < _entitiesToDisplay; i++)
            {
                _leaderboardEntityDisplays[i].transform.SetSiblingIndex(i);
                _leaderboardEntityDisplays[i].UpdateText();
                bool shouldShow = i <= _entitiesToDisplay - 1;
                _leaderboardEntityDisplays[i].gameObject.SetActive(shouldShow);
            }

            var localLeaderboardEntity = _leaderboardEntityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

            if (localLeaderboardEntity != null)
            {
                if (localLeaderboardEntity.transform.GetSiblingIndex() >= _entitiesToDisplay)
                {
                    _leaderboardEntityHolder.GetChild(_entitiesToDisplay - 1).gameObject.SetActive(false);
                    localLeaderboardEntity.gameObject.SetActive(true);
                }
            }
        }

        void HandleOnPlayerSpawn(TankPlayerController value)
        {
            _leaderboardEntityStates.Add(new LeaderboardEntityState()
            {
                ClientId = value.OwnerClientId,
                PlayerName = value.PlayerName.Value,
                Coin = 0
            });

            value.CoinCollectHandler.CoinTotal.OnValueChanged += (int oldCoin, int newCoin) => HandleOnCoinValueChanged(value.OwnerClientId, newCoin);
        }

        void HandleOnPlayerDespawn(TankPlayerController value)
        {
            if (_leaderboardEntityStates == null || value == null) return;
            
            //TODO this code has bug will fixes
            foreach (var entity in _leaderboardEntityStates)
            {
                if (entity.ClientId != value.OwnerClientId) continue;
            
                _leaderboardEntityStates.Remove(entity);
                break;
            }
            
            value.CoinCollectHandler.CoinTotal.OnValueChanged -= (int oldCoin, int newCoin) => HandleOnCoinValueChanged(value.OwnerClientId, newCoin);
        }
        
        void HandleOnCoinValueChanged(ulong clientId, int newCoin)
        {
            for (int i = 0; i < _leaderboardEntityStates.Count; i++)
            {
                if(_leaderboardEntityStates[i].ClientId != clientId) continue;

                _leaderboardEntityStates[i] = new LeaderboardEntityState()
                {
                    ClientId = _leaderboardEntityStates[i].ClientId,
                    PlayerName = _leaderboardEntityStates[i].PlayerName,
                    Coin = newCoin
                };

                break;
            }
        }
    }
}