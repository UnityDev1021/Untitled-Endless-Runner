#define MOBILE_MODE

using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;

namespace Untitled_Endless_Runner
{
    public class PlayGameServices : MonoBehaviour
    {
        [SerializeField] private TMP_Text signInStatus;
        private bool signedIn;

        [Header("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
#if MOBILE_MODE
            localGameLogic.OnGameOver += PostScoreToLeaderBoard;
#endif
        }

        private void OnDisable()
        {
#if MOBILE_MODE
            localGameLogic.OnGameOver -= PostScoreToLeaderBoard;
#endif
        }

        async void Start()
        {
#if MOBILE_MODE
            try
            {
                await UnityServices.InitializeAsync();
                LoginGooglePlayGames();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
        }

        //On the Sign In button, under the settings panel on the Main Menu
        public void LoginGooglePlayGamesManually()
        {
            if (!signedIn)
            {
                // Initialize next line under PlayGamesPlatform from within Awake
                PlayGamesPlatform.Activate();
                PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
            }
        }

        private void LoginGooglePlayGames()
        {
            // Initialize next line under PlayGamesPlatform from within Awake
            PlayGamesPlatform.Activate();
            // PlayGamesPlatform.Instance.Authenticate((success) =>
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }

        private string tokenGPGS;
        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
                PlayGamesPlatform.Instance.RequestServerSideAccess(/* forceRefreshToken= */ true, async code =>
                {
                    //Debug.Log("Authorization code: " + code);
                    tokenGPGS = code;
                    // send code to server          // This token serves as an example to be used for SignInWithGooglePlayGames                    
                    await SignInWithGooglePlayGamesAsync(tokenGPGS);
                });
            }
            else
            {
                // Disable your integration with Play Games Services or show a login button
                // to ask users to sign-in. Clicking it should call
                //debugText.text = "Status : " + status.ToString();
                Debug.Log($"Not Successful Google Play Games  Sign in. Error Details : {status}");
#if UNITY_ANDROID && !UNITY_EDITOR
                _ShowAndroidToastMessage($"Error Signing In, Status : {status}");
#endif
            }
        }

        async Task SignInWithGooglePlayGamesAsync(string authCode)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
                signInStatus.text = $"Sign Out";
                signedIn = true;

                //debugText.text = "Status : Google Play Games Successful Sign in";
                Debug.Log("SignIn is successful.");
            }
            catch (Unity.Services.Authentication.AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
#if UNITY_ANDROID && !UNITY_EDITOR
                _ShowAndroidToastMessage($"Error Signing In : {ex.Message}");
#endif
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
#if UNITY_ANDROID && !UNITY_EDITOR
                _ShowAndroidToastMessage($"Error Signing In : {ex.Message}");
#endif
            }
        }

        //On the Sign Out button, under the Settings Panel on the Main Menu
        public void SignOutOfServices()
        {
            if (signedIn)
            {
                if (!AuthenticationService.Instance.IsSignedIn) return;
                if (PlayGamesPlatform.Instance.IsAuthenticated())
                {
                    signedIn = false;
                    signInStatus.text = $"Sign In";
                    AuthenticationService.Instance.SignOut();
                    Debug.Log("Signed Out");
                }
            }
        }

        private void PostScoreToLeaderBoard(int score)
        {
            Social.ReportScore(score, "CgkIp9_L4-cFEAIQAg", (bool success) =>
            {
                if (success)
                    Debug.Log($"Successfully Added Score to LeaderBoard : {score}");
                else
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    _ShowAndroidToastMessage($"Unable To Add Score To LeaderBoard");
#endif
                    Debug.Log($"Unable To Add Score To LeaderBoard : {score}");
                }
            });
        }

        public void ShowLeaderBoard()
        {
            Social.ShowLeaderboardUI();
        }

        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}