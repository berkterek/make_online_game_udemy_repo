using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MakeOnlineGame.Controllers
{
    public class NameSelectController : MonoBehaviour
    {
        string PLAYER_NAME_KEY = "PlayerName";
        
        [SerializeField] TMP_InputField _nameInputField;
        [SerializeField] Button _connectButton;
        [SerializeField] int _minNameLength = 1;
        [SerializeField] int _maxNameLength = 12;

        void Start()
        {
            //if its server pass this process
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) return;
            
            _nameInputField.text = PlayerPrefs.GetString(PLAYER_NAME_KEY, string.Empty);
        }

        void OnEnable()
        {
            _connectButton.onClick.AddListener(HandleOnButtonClicked);
            _nameInputField.onValueChanged.AddListener(HandleOnInputTextValueChanged);
        }

        void OnDisable()
        {
            _connectButton.onClick.RemoveListener(HandleOnButtonClicked);
            _nameInputField.onValueChanged.RemoveListener(HandleOnInputTextValueChanged);
        }

        void HandleOnButtonClicked()
        {
            _nameInputField.interactable = false;
            _connectButton.interactable = false;
            
            var userName = _nameInputField.text;
            PlayerPrefs.SetString(PLAYER_NAME_KEY, userName);
            
            WaitAndLoadNextSceneAsync();
        }

        void HandleOnInputTextValueChanged(string value)
        {
            _connectButton.interactable = value.Length > _minNameLength && value.Length < _maxNameLength;
        }

        async UniTask WaitAndLoadNextSceneAsync()
        {
            await UniTask.Delay(2000);
            await SceneManager.LoadSceneAsync("NetBootstrap");
        }
    }
}