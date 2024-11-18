using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }
        else if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return AuthState;
        }

        await SignInAnon(maxTries);


        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(1000);
        }
        return AuthState;
    }

    private static async Task SignInAnon(int maxTries)
    {
        int tries = 0;
        AuthState = AuthState.Authenticating;
        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // login with UGS
            }
            catch (AuthenticationException AE)
            {
                AuthState = AuthState.Error;
                Debug.LogError(AE.Message);
            }
            catch (RequestFailedException RFE)
            {
                AuthState = AuthState.Error;
                Debug.LogError(RFE.Message);
            }


            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            { // check that we are correctly in
                AuthState = AuthState.Authenticated;
                break;
            }

            tries++;
            await Task.Delay(1000);
        }

        if (AuthState != AuthState.Authenticated)
        {
            AuthState = AuthState.TimeOut;
            Debug.LogWarning("Signung unsuccesful");
        }
    }
}




public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
