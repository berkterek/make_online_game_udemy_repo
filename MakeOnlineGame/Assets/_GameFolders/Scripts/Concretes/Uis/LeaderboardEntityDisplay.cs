using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Uis
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text _text;
        [SerializeField] Color _color;

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

            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                _text.color = _color;
            }

            UpdateCoin(coin);
        }

        public void UpdateCoin(int coin)
        {
            Coin = coin;
            
            UpdateText();
        }

        public void UpdateText()
        {
            _text.SetText($"{transform.GetSiblingIndex() + 1}. {_playerName}:{Coin}");
        }
    }
}