using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using System.Security.Authentication;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace Untitled_Endless_Runner
{
    public class PlayGameServices : MonoBehaviour
    {
        [SerializeField] private TMP_Text debugText;

        [Header("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic.OnGameOver += PostScoreToLeaderBoard;
        }

        private void OnDisable()
        {
            localGameLogic.OnGameOver -= PostScoreToLeaderBoard;
        }


        // Start is called before the first frame update
        void Start()
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }

        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
                debugText.text = "Status : Successful Sign in";
                Debug.Log($"Successful Sign in"); 
                
                PlayGamesPlatform.Instance.RequestServerSideAccess(
                /* forceRefreshToken= */ false, code=>
                                         {
                                             Debug.Log("Authorization code: " + code);
                                             // send code to server
                                         });
            }
            else
            {
                // Disable your integration with Play Games Services or show a login button
                // to ask users to sign-in. Clicking it should call
                // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
                debugText.text = "Status : " + status.ToString();
                Debug.Log($"Not Successful Sign in. Error Details : {status}");
            }
        }

        /*private async Task SignInWithGooglePlayGamesAsync(string authCode)
        {
            try
            {

                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
                Debug.Log("SignIn is successful.");
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }*/

        private void PostScoreToLeaderBoard(int score)
        {
            Social.ReportScore(score, "CgkI8bj98OQMEAIQAg", (bool success) =>
            {
                if (success)
                    debugText.text = $"Successfully Added Score to LeaderBoard : {score}";
                else
                    debugText.text = $"Not Successfull : {score}";
            });
        }

        public void ShowLeaderBoard()
        {
            Social.ShowLeaderboardUI();
        }
    }
}