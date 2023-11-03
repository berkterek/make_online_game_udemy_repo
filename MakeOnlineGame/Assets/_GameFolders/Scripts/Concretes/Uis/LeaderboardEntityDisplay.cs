using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace MakeOnlineGame.Uis
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text _text;

        FixedString32Bytes _playerName;
        
        public ulong ClientId { get; private set; }
        public int Coin { get; private set; }

        void OnValidate()
        {
            if (_text == null) _text = GetComponent<TMP_Text>();
        }

        public void SetData(ulong clientId, FixedString32Bytes playerName, int coin)
        {
            ClientId = clientId;
            _playerName = playerName;

            UpdateCoin(coin);
        }

        public void UpdateCoin(int coin)
        {
            Coin = coin;
            
            UpdateText();
        }

        void UpdateText()
        {
            _text.SetText($"{_playerName}:{Coin}");
        }
    }
}