using UnityEngine;
//using Unity.Services.Authentication;
using Unity.Services.PlayerAccounts;
//using Unity.Services.Core;
using System.Threading.Tasks;

public class UGSM : MonoBehaviour
{
    private string sesToken;

    // INITIALIZE UNITYSERVICES... GET ACCESS TOKEN FOR UNITY LINKING //
    private async void Start()
    {
        /*await UnityServices.InitializeAsync();

        print($"Unity services initialization: {UnityServices.State}");
        print($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

        AuthenticationService.Instance.SignedIn += () =>
        {
            print($"PlayedID: {AuthenticationService.Instance.PlayerId}");

            sesToken = AuthenticationService.Instance.AccessToken;
            print($"Access Token: {sesToken}");

            print("Sign in succeeded.");
        };
        AuthenticationService.Instance.SignedOut += () =>
        {
            print("Signed Out!");
        };
        AuthenticationService.Instance.SignInFailed += errorResponse =>
        {
            print($"Sign in anonymously failed with error code: {errorResponse.ErrorCode}");
        };


        PlayerAccountService.Instance.SignedIn += () =>
        {
            print($"Signed in with Player Account");
        };*/

    }

    // THIS METHOD IS HOOKED UP TO A BUTTON IN THE CANVAS... //
    public async void SignInAnonymously()
    {/*
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            print("Sign in anonymously succeeded!");
        }
        catch (AuthenticationException ex)
        {
            print(ex);
        }
        catch (RequestFailedException ex)
        {
            print(ex);
        }*/
    }


    public void SignInWithUnity_ButtonClicked()
    {/*
        string accessToken = AuthenticationService.Instance.AccessToken;
        print($"Access Token : {accessToken}");

        Task playerAccountSignIn = SignInWithUnityAsync(accessToken);*/
    }
    public async Task SignInWithUnityAsync(string accessToken)
    {/*
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");
            
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }*/
    }



    // THIS IS HOOKED UP TO ANOTHER BUTTON... AND IS INTENDED //
    // TO BE PRESSED AFTER THE ANONYMOUS SIGN IN... HOWEVER //
    // IT DOESN'T WANT TO WORK WITH ME... ANY IDEAS? //
    public async void LinkWithUnity()
    {/*
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(sesToken);
            Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }

        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }*/
    }
}
