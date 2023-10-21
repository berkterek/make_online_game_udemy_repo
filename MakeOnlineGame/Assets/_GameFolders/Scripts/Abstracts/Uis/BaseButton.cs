using UnityEngine;
using UnityEngine.UI;

namespace MakeOnlineGame.Abstracts.Uis
{
    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField] protected Button _button;
        
        protected virtual void OnValidate()
        {
            if (_button == null) _button = GetComponentInChildren<Button>();
        }

        protected virtual void OnEnable()
        {
            _button.onClick.AddListener(HandleOnButtonClicked);
        }

        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnButtonClicked);
        }

        protected abstract void HandleOnButtonClicked();
    }
}