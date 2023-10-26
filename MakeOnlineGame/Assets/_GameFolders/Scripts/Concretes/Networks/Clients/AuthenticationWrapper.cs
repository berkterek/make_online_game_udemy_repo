using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace MakeOnlineGame.Networks.Clients
{
    public static class AuthenticationWrapper
    {
        public static AuthenticateState AuthenticateState { get; private set; } = AuthenticateState.NotAuthenticated;

        public static async UniTask<AuthenticateState> DoAuthenticate(int maxTry = 5)
        {
            if (AuthenticateState == AuthenticateState.Auhenticated) return AuthenticateState;

            int tryValue = 0;
            
            AuthenticateState = AuthenticateState.Authenticating;
            
            while (AuthenticateState == AuthenticateState.Authenticating && tryValue < maxTry)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthenticateState = AuthenticateState.Auhenticated;
                }

                Debug.Log(AuthenticateState);
                
                tryValue++;

                await UniTask.Delay(1000);
            }

            return AuthenticateState;
        }
    }

    public enum AuthenticateState : byte
    {
        NotAuthenticated,
        Authenticating,
        Auhenticated,
        Error,
        Timeout
    }
}

