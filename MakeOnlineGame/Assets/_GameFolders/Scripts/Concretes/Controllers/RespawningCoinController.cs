namespace MakeOnlineGame.Controllers
{
    public class RespawningCoinController : BaseCoinController
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

            return base.Collect();
        }
    }
}