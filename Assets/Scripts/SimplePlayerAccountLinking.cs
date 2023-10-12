using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.PlayerAccounts;
using Unity.VisualScripting;

public class SimplePlayerAccountLinking : MonoBehaviour
{

    #region UI GAMEOBJECTS TO WIRE UP //////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField]
    public Text m_Authenticated;
    [SerializeField]
    public Text m_PlayerID;
    [SerializeField]
    public InputField m_PlayerNameInput;
    [SerializeField]
    public Button m_PlayerNameSaveButton;   
    [SerializeField]
    public Button m_SignInButton;
    [SerializeField]
    public Button m_SignOutButton;
    [SerializeField]
    public Button m_LinkButton;
    [SerializeField]
    public Button m_UnLinkButton;
    #endregion


    #region UNITY GAMEOBJECT LIFECYCLE EVENTS /////////////////////////////////////////////////////////////////////////////////////
    async void Start()
    {
        // INITIALIZE UNITY GAMING SERVICES
        await UnityServices.InitializeAsync();
        Debug.Log($"Unity Services : {UnityServices.State}");

        // REGISTER PLAYER ACCOUNT HANDLERS
        PlayerAccountService.Instance.SignedIn += OnPlayerAccountSignedIn;
        PlayerAccountService.Instance.SignedOut += OnPlayerAccountSignedOut;
        PlayerAccountService.Instance.SignInFailed += OnPlayerAccountSignInFailed;

        // REGISTER AUTHENTICATION SERVICE HANDLERS
        AuthenticationService.Instance.SignedIn += OnAuthSignedIn;
        AuthenticationService.Instance.SignedOut += OnAuthSignedOut;
        AuthenticationService.Instance.SignInFailed += OnAuthSignInFailed;
        AuthenticationService.Instance.Expired += OnAuthSignInExpired;

        // SIGN IN ANONYMOUSLY IF SESSION TOKEN EXISTS (FORCES PLAYER TO SIGN IN)
        if (AuthenticationService.Instance.SessionTokenExists) // TODO ADD TOKEN EXPIRY HANDLING
        {
            Debug.Log("Authentication Session Token Exixts. Signing in anonymously...");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        if (AuthenticationService.Instance.PlayerName == null)
        {
            Debug.Log("This must be a new player, their player name is NULL");
        }
        Debug.Log($"Authentication Is Signed In : {AuthenticationService.Instance.IsSignedIn}");
    }

    private void Destory()
    {
        // UN-REGISTER PLAYER ACCOUNT SIGN IN HANDLERS
        PlayerAccountService.Instance.SignedIn -= OnPlayerAccountSignedIn;
        PlayerAccountService.Instance.SignedOut -= OnPlayerAccountSignedOut;
        PlayerAccountService.Instance.SignInFailed -= OnPlayerAccountSignInFailed;

        // UN-REGISTER AUTHENTICATION  HANDLERS
        AuthenticationService.Instance.SignedIn -= OnAuthSignedIn;
        AuthenticationService.Instance.SignedOut -= OnAuthSignedOut;
        AuthenticationService.Instance.SignInFailed -= OnAuthSignInFailed;
        AuthenticationService.Instance.Expired -= OnAuthSignInExpired;
    }
    #endregion


    #region UPDATE UI //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private async void UpdateUI()
    {
        // UPDATE AUTHENTICATION STATUS & PLAYER ID TEXT
        m_Authenticated.text = AuthenticationService.Instance.IsSignedIn ? "Authenticated":"Not Authenticatied";
        m_PlayerID.text = AuthenticationService.Instance.PlayerId != null ? AuthenticationService.Instance.PlayerId : ""; 

        // UPDATE UNITY PLAYER ACCOUNT BUTTONS
        bool isPlayerAccountSignedIn = PlayerAccountService.Instance.IsSignedIn; 
        Debug.Log($"Player Account Signed In : {isPlayerAccountSignedIn}");
        m_SignInButton.interactable = !isPlayerAccountSignedIn;
        m_SignOutButton.interactable = isPlayerAccountSignedIn;

        // UPDATE UNITY PLAYER ACCOUNT LINK BUTTONS
        PlayerInfo playerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();
        bool isLinkedToPlayerAccounts = playerInfo.Identities.Find((Identity i) => i.TypeId.ToLowerInvariant().Equals("unity")) != null;
        Debug.Log($"Player Account Linked : {isLinkedToPlayerAccounts}");
        m_LinkButton.interactable = !isLinkedToPlayerAccounts && isPlayerAccountSignedIn;
        m_UnLinkButton.interactable = isLinkedToPlayerAccounts;

        // UPDATE PLAYER NAME FIELD DISPLAY
        string playerName = AuthenticationService.Instance.PlayerName;
        if (playerName == null) 
        {
            playerName = GetPlayerNameFromPlayer(); // ALLOW USER TO PROVIDE THEIR OWN NAME
            if (playerName != null)
            {
                // SAVE THE NAME PROVIDED BY THE PLAYER
                await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            }
            else
            {
                // GENERATE RANDOM NAME IF PLAYER DIDN'T PROVIDE ONE
                playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            }
        }

        Debug.Log($"Player Name : {playerName}");
        // NOTE - REPLACING "_" with " " AND REMOVING THE NUMERICAL SUFFIX "#1234" TO IMPROVE READBILITY
        // BEWARE IF YOU ALLOW PLAYERS TO SET NAMES WITH "_" OR "#" IN THEM!!!
        m_PlayerNameInput.text = playerName?.Replace("_"," ").Split("#")[0] ;
    }
    #endregion

    private string GetPlayerNameFromPlayer()
    {
        // GET NAME INPUT FROM PLAYER

        // TEST BY COMMENTING IN/OUT ONE OF THESE LINES 
        string playerName = null;
        //string playerName = "New_Player";

        return playerName;
    }
    #region UI EVENT HANDLERS /////////////////////////////////////////////////////////////////////////////////////////////////////
    public async void SignIn_Button_Clicked()
    {
        // START PLAYER ACCOUNTS SIGN IN PROCESS
        Debug.Log("Player Accounts - Sign In Button Clicked");
        await PlayerAccountService.Instance.StartSignInAsync();       
    }

    public void SignOut_Button_Clicked()
    {
        // START PLAYER ACCOUNTS SIGN OUT
        Debug.Log("Player Accounts - Sign Out Button Clicked");
        PlayerAccountService.Instance.SignOut();        
    }

    public async void Link_Button_Clicked()
    {
        // LINK TO UNITY PLAYER ACCOUNT
        Debug.Log("Link Button Clicked");
        await AuthenticationService.Instance.LinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        UpdateUI();
    }

    public async void UnLink_Button_Clicked()
    {
        // UNLINK FROM UNITY PLAYER ACCOUNT
        Debug.Log("UnLink Button Clicked");
        await AuthenticationService.Instance.UnlinkUnityAsync();
        UpdateUI();
    }

    public async void PlayerName_Save_Button_Clicked() 
    { 
        // GET PLAYER NAME, TRIM TO 50 CHARS AND REPLACE ANY SPACES WITH '_'
        string m_playerName = m_PlayerNameInput.text?.Replace(" ", "_"); 
        m_playerName = m_playerName.Length > 50 ? m_playerName[..50] : m_playerName;
        Debug.Log($"Saving Player Name : {m_playerName}");

        if (AuthenticationService.Instance.IsSignedIn)
        {           
            await AuthenticationService.Instance.UpdatePlayerNameAsync(m_playerName);
        }
    }
    #endregion


    #region PLAYER ACCOUNTS ACTION HANDLERS //////////////////////////////////////////////////////////////////////////////////////
    private void OnPlayerAccountSignedIn()
    {
        // UNITY PLAYER ACCOUNTS SIGN IN SUCCESSFULL
        string accessToken = PlayerAccountService.Instance.AccessToken;
        if (!string.IsNullOrEmpty(accessToken)) {
            Debug.Log($"Attempting SignIn with PlayerAccounts Access Token - {accessToken}");
            AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
        }
        UpdateUI();
    }

    private void OnPlayerAccountSignedOut()
    {       
        UpdateUI();
    }

    private void OnPlayerAccountSignInFailed(RequestFailedException requestFailedException)
    {
        // UNITY PLAYER ACCOUNT SIGN IN FAILED
        Debug.Log($"Player Account Sign In Error : {requestFailedException.ErrorCode} : {requestFailedException.Message}");
        UpdateUI();
    }
    #endregion


    #region AUTHENTICATION SERVICE ACTION HANDLERS //////////////////////////////////////////////////////////////////////////////
    private void OnAuthSignedIn()
    {
        // AUTHENTICATION SIGNED IN SUCCESSFULLY
        Debug.Log($"Authentication Service Signed In with PlayerID - {AuthenticationService.Instance.PlayerId}");
        UpdateUI();
    }

    private void OnAuthSignedOut()
    {
        // AUTHENTICATION SIGNED OUT
        Debug.Log("Authentication Service Signed Out");
        UpdateUI();
    }

    private void OnAuthSignInFailed(RequestFailedException requestFailedException)
    {
        // AUTHENTICATION SIGNED IN FAILED
        Debug.Log($"Authentication SignIn Failed : {requestFailedException.ErrorCode} : {requestFailedException.Message}");
        UpdateUI();
    }

    private void OnAuthSignInExpired()
    {
        // AUTHENTICATION TOKEN EXPIRED
        Debug.Log("Authentication Service Token Expired");
        UpdateUI();
    }
    #endregion
}
