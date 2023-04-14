using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject _loggedInPanel;
    [SerializeField] private TextMeshProUGUI _statText;
    // Bottom Panel Buttons
    [SerializeField] private GameObject _signInLastChachedUserBtn;
    [SerializeField] private GameObject _clearCachedUserBtn;
    [SerializeField] private GameObject _signOutBtn;
    [SerializeField] private GameObject _showStatusBtn;
    private bool _isShowingPanel;
    private string _service = "";
    private string _triedService = "";
    void Awake()
    {
        _statText.text = "No log-in events registered... " + "\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\nx\n";
        _loggedInPanel.SetActive(false);
        _isShowingPanel = false;
    }
    // async Start()
    async void Start()
    {
        #region InitializingUnityServices
        // UnityServices.InitializeAsync() will initialize all services that are subscribed to Core.
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);
        // (optional) set up events
        SetupEvents();
        #endregion
    }
    #region Setup/Register authentication events
    // Setup authentication event handlers if desired
    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            _signOutBtn.SetActive(true);

        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
            _signOutBtn.SetActive(false);
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }
    #endregion

    #region SignInAnonymouslyAsync
    public async void SignInAnon()
    {
        // set service
        _triedService = "Anonymous Log-In";
        await SignInAnonymouslyAsync();
    }
    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Set Logged In panel Active and change status

            OnSuccessfulLogIn(out _service, _triedService);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            OnUnsuccessfulLogIn(ex.ToString(), _triedService);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            OnUnsuccessfulLogIn(ex.ToString(), _triedService);
        }
    }
    #endregion

    #region SignInGPGS
    public string TokenGPGS;
    public string ErrorGPGS;
    public void LoginGPGSManually()
    {
        // set service
        _triedService = "Google Play Games";
        LoginGooglePlayGames();
    }
    private void LoginGooglePlayGames()
    {
        // Initialize next line under PlayGamesPlatform from within Awake
        PlayGamesPlatform.Activate();
        // PlayGamesPlatform.Instance.Authenticate((success) =>
        PlayGamesPlatform.Instance.ManuallyAuthenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
                {
                    Debug.Log("Authorization code: " + code);
                    TokenGPGS = code;
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                    await SignInWithGooglePlayGamesAsync(TokenGPGS);
                });
            }
            else if (success == SignInStatus.Canceled)
            {
                ErrorGPGS = "Failed to retrieve Google play games authorization code due to Sign-In Cancelation";
                Debug.Log("Login Unsuccessful Due to Sign-In Cancelation");
                OnUnsuccessfulLogIn(ErrorGPGS, _triedService);
            }
            else
            {
                ErrorGPGS = "Failed to retrieve Google play games authorization code due to Internal Error";
                Debug.Log("Login Unsuccessful Due to Internal Error");
                OnUnsuccessfulLogIn(ErrorGPGS, _triedService);
            }
        });
    }

    async Task SignInWithGooglePlayGamesAsync(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log("SignIn is successful.");
            OnSuccessfulLogIn(out _service, _triedService);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            OnUnsuccessfulLogIn(ex.ToString(), _triedService);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            OnUnsuccessfulLogIn(ex.ToString(), _triedService);
        }
    }
    #endregion

    #region SignInGoogle
    public string TokenGoogle;
    public string ErrorGoogle;
    public void LoginGoogleManually()
    {
        _triedService = "Google";
        LoginGooglePlayGamesForGoogle();
    }

    private void LoginGooglePlayGamesForGoogle()
    {
        // Initialize next line under PlayGamesPlatform from within Awake
        PlayGamesPlatform.Activate();
        // PlayGamesPlatform.Instance.Authenticate((success) =>
        PlayGamesPlatform.Instance.ManuallyAuthenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games for Google successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
                {
                    Debug.Log("Authorization code: " + code);
                    TokenGoogle = code;
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                    await SignInWithGoogleAsync(TokenGoogle);
                });
            }
            else if (success == SignInStatus.Canceled)
            {
                ErrorGoogle = "Failed to retrieve Google play games for Google authorization code due to Sign-In Cancelation";
                Debug.Log("Login Unsuccessful Due to Sign-In Cancelation");
                OnUnsuccessfulLogIn(ErrorGoogle, _triedService);
            }
            else
            {
                ErrorGoogle = "Failed to retrieve Google play games for Google authorization code due to Internal Error";
                Debug.Log("Login Unsuccessful Due to Internal Error");
                OnUnsuccessfulLogIn(ErrorGoogle, _triedService);
            }
        });
    }
    async Task SignInWithGoogleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGoogleAsync(idToken);
            Debug.Log("SignIn is successful.");
            OnSuccessfulLogIn(out _service, _triedService);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            OnUnsuccessfulLogIn(ex.ToString(), _triedService);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            OnUnsuccessfulLogIn(ex.ToString(), _triedService);
            Debug.LogException(ex);
        }
    }
    #endregion SignInGoogle

    private void OnSuccessfulLogIn(out string _service, string triedservice)
    {
        _loggedInPanel.SetActive(true);
        _isShowingPanel = true;
        _showStatusBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Hide Status Panel";
        _service = triedservice;
        _statText.text = $"Status: Logged-In with {triedservice}\n";
        _statText.color = Color.green;
        _signOutBtn.SetActive(true);
    }
    private void OnUnsuccessfulLogIn(string ex, string triedservice)
    {
        _loggedInPanel.SetActive(true);
        _isShowingPanel = true;
        if (_statText.text.Contains("No log-in events registered..."))
        {
            _statText.text = "";
        }
        _statText.text += $"\nStatus: Unable to log into {triedservice}\n";
        _statText.text += $"Error:\n{ex}\n";
        _showStatusBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Hide Status Panel";
        _statText.color = Color.red;
    }
    public void SignOutOfServices()
    {
        if (!AuthenticationService.Instance.IsSignedIn) return;
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            // Todo:
        }
        _statText.text = $"Status: Logged out of {_service}\n";
        _statText.color = Color.white;
        AuthenticationService.Instance.SignOut();
        Debug.Log("Signed Out");
        _signOutBtn.SetActive(false);
    }
    public void ClearCachedUser()
    {
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            AuthenticationService.Instance.ClearSessionToken();
        }
    }

    public void ShowStatus()
    {
        if (_isShowingPanel)
        {
            _loggedInPanel.SetActive(false);
            _isShowingPanel = false;
            _showStatusBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Show Status Panel";
        }
        else
        {
            _loggedInPanel.SetActive(true);
            _isShowingPanel = true;
            _showStatusBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Hide Status Panel";
        }
    }

    public void SignInCachedUserManually()
    {
        if (!_service.Contains("Cached"))
        {
            _triedService = _service + " as Cached User";
        }
        else
        {
            _triedService = _service;
        }
        SignInCachedUser();
    }
    async void SignInCachedUser()
    {
        // Check if a cached user already exists by checking if the session token exists
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            // if not, then do nothing
            OnUnsuccessfulLogIn("Error: No previous logged-in session token exists", "Cached User Sign-In");
            return;
        }

        // Sign in Anonymously
        // This call will sign in the cached user.
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            OnSuccessfulLogIn(out _service, _triedService);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            OnUnsuccessfulLogIn(ex.ToString(), _service);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            OnUnsuccessfulLogIn(ex.ToString(), _service);
        }
    }
}
