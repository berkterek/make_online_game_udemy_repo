using UnityEngine;

namespace MakeOnlineGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] int _frame = 60;
        
        void Start()
        {
            Application.targetFrameRate = _frame;
        }
    }    
}

