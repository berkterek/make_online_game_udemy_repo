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

            await SingInAnonymouslyAsync(maxTry);

            if (AuthenticateState == AuthenticateState.Authenticating ||
                AuthenticateState == AuthenticateState.NotAuthenticated)
            {
                await AuthenticatingAsync();
                return AuthenticateState;
            }

            return AuthenticateState;
        }

        private static async UniTask<AuthenticateState> AuthenticatingAsync()
        {
            while (AuthenticateState == AuthenticateState.Authenticating ||
                   AuthenticateState == AuthenticateState.NotAuthenticated)
            {
                await UniTask.Delay(200);
            }

            return AuthenticateState;
        }

        private static async UniTask SingInAnonymouslyAsync(int maxTry)
        {
            AuthenticateState = AuthenticateState.Authenticating;            
            
            int tryValue = 0;
            
            while (AuthenticateState == AuthenticateState.Authenticating && tryValue < maxTry)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        AuthenticateState = AuthenticateState.Auhenticated;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                    AuthenticateState = AuthenticateState.Error;
                    break;
                }

                tryValue++;

                await UniTask.Delay(1000);
            }
            
            if (AuthenticateState != AuthenticateState.Auhenticated)
            {
                AuthenticateState = AuthenticateState.Timeout;
                Debug.Log($"Player was not signed in successfully try value: {tryValue}");
            }
            
            Debug.Log(AuthenticateState);
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

