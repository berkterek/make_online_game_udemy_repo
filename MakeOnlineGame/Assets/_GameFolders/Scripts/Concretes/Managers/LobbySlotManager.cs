using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MakeOnlineGame.Controllers;
using MakeOnlineGame.Networks.Clients;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace MakeOnlineGame.Managers
{
    public class LobbySlotManager : MonoBehaviour
    {
        [SerializeField] Transform _lobbySlotParent;
        [SerializeField] LobbySlotController _slotPrefab;

        bool _isJoining = false;
        bool _isRefreshing = false;

        void OnEnable()
        {
            RefreshListAsync();
        }

        public async void RefreshListAsync()
        {
            if (_isRefreshing) return;

            _isRefreshing = true;

            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                //top 25 lobby
                options.Count = 25;

                //available slots is gt(greater than) 0(zero)
                //is locked equal to 0(zero) => not show this lobby
                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT,
                        value: "0"),
                    new QueryFilter(field: QueryFilter.FieldOptions.IsLocked, op: QueryFilter.OpOptions.EQ,
                        value: "0"),
                };

                QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

                foreach (Transform child in _lobbySlotParent)
                {
                    Destroy(child.gameObject);
                }

                foreach (var lobby in lobbies.Results)
                {
                    var newSlot = Instantiate(_slotPrefab, _lobbySlotParent);
                    newSlot.Initialized(this, lobby);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            _isRefreshing = false;
        }

        public async UniTask JoinAsync(Lobby lobby)
        {
            if (_isJoining) return;

            _isJoining = true;
            try
            {
                var joinLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                string joinCode = joinLobby.Data["JoinCode"].Value;
                await ClientSingleton.Instance.ClientManager.StartClientAsync(joinCode);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

            _isJoining = false;
        }
    }
}