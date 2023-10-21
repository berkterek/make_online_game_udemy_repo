using System;
using MakeOnlineGame.Abstracts.Uis;
using MakeOnlineGame.Enums;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace MakeOnlineGame.Uis
{
    public class ConnectButton : BaseButton
    {
        [SerializeField] ConnectType _connectType;
        [SerializeField] TMP_Text _valueText;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (_valueText == null) _valueText = GetComponentInChildren<TMP_Text>();
        }

        void Start()
        {
            _valueText.SetText(_connectType.ToString());
        }

        protected override void HandleOnButtonClicked()
        {
            switch (_connectType)
            {
                case ConnectType.Host:
                    NetworkManager.Singleton.StartHost();
                    break;
                case ConnectType.Client:
                    NetworkManager.Singleton.StartClient();
                    break;
                case ConnectType.Server:
                    NetworkManager.Singleton.StartServer();
                    break;
            }
        }
    }
}