using MakeOnlineGame.Managers;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class LobbySlotController : MonoBehaviour
    {
        [SerializeField] TMP_Text _lobbyNameText;
        [SerializeField] TMP_Text _lobbyPlayerText;

        LobbySlotManager _lobbySlotManager;
        Lobby _lobby;
        
        public void Initialized(LobbySlotManager lobbySlotManager, Lobby lobby)
        {
            _lobbySlotManager = lobbySlotManager;

            _lobby = lobby;
            _lobbyNameText.SetText(lobby.Name);
            _lobbyPlayerText.SetText($"{lobby.Players.Count}/{lobby.MaxPlayers}");
        }

        public async void Join()
        {
            await _lobbySlotManager.JoinAsync(_lobby);
        }
    }
}