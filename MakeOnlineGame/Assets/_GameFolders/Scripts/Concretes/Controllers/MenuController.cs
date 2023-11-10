using System;
using MakeOnlineGame.Networks.Clients;
using MakeOnlineGame.Networks.Hosts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MakeOnlineGame.Controllers
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] TMP_InputField _joinCodeInputField;
        [SerializeField] TMP_Text _timer;
        [SerializeField] TMP_Text _searhingText;
        [SerializeField] Button _hostButton;
        [SerializeField] Button _clientButton;
        [SerializeField] Button _findMatch;

        void Start()
        {
            if (ClientSingleton.Instance == null) return;
            
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            _timer.SetText("");
            _searhingText.SetText("");
        }

        void OnEnable()
        {
            _hostButton.onClick.AddListener(HandleOnHostButtonClicked);
            _clientButton.onClick.AddListener(HandleOnClientButtonClicked);
            _findMatch.onClick.AddListener(HandleOnFindMatch);
        }

        void OnDisable()
        {
            _hostButton.onClick.RemoveListener(HandleOnHostButtonClicked);
            _clientButton.onClick.RemoveListener(HandleOnClientButtonClicked);
            _findMatch.onClick.RemoveListener(HandleOnFindMatch);
        }
        
        async void HandleOnHostButtonClicked()
        {
            await HostSingleton.Instance.HostManager.StartHostAsync();
        }
        
        async void HandleOnClientButtonClicked()
        {
            await ClientSingleton.Instance.ClientManager.StartClientAsync(_joinCodeInputField.text);
        }
        
        void HandleOnFindMatch()
        {
            
        }
    }    
}

