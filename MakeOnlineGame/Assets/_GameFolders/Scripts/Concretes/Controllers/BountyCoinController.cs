
namespace MakeOnlineGame.Controllers
{
    public class BountyCoinController : BaseCoinController
    {
        public override int Collect()
        {
            if (!IsServer)
            {
                ShowDisplay(false);
                return 0;
            }

            if (_isAlreadyCollected) return 0;

            _isAlreadyCollected = true;
            
            Destroy(this.gameObject);
            
            return base.Collect();
        }
    }    
}

