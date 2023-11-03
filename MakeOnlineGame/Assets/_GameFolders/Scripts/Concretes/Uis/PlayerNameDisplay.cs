using System;
using MakeOnlineGame.Controllers;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace MakeOnlineGame.Uis
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text _playerNameText;
        [SerializeField] TankPlayerController _tankPlayController;

        void OnEnable()
        {
            _tankPlayController.PlayerName.OnValueChanged += HandleOnValueChanged;
        }

        void OnDisable()
        {
            _tankPlayController.PlayerName.OnValueChanged -= HandleOnValueChanged;
        }

        void Start()
        {
            if (string.IsNullOrEmpty(_playerNameText.text))
            {
                _playerNameText.SetText(_tankPlayController.PlayerName.Value.Value);    
            }
        }

        void HandleOnValueChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            _playerNameText.SetText(newValue.Value);
        }
    }
}