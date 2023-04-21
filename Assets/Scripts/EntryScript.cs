//#define SKIP_ENTRY                          //For Testing#define SKIP_ENTRY                          //For Testing
//#define ENABLE_PORTAL                     //Disabed bc not using

using System.Collections;
//using UnityEditor.Animations;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class EntryScript : MonoBehaviour
    {
        [SerializeField] private Animator playerAnimator, backgroundAnimator;               //portalAnimator, 
        //[SerializeField] private AnimatorController[] backgroundAnimatorControllers;            //Cant use outside Unity Editor

        [Header("Local Refernece Scripts")]
        [SerializeField] private GameLogic localGameLogic;
        [SerializeField] private BackGroundController localBG_Controller;

        [Header("Disabled GameObjects")]
        [SerializeField] private GameObject[] disabledObjects;
        [SerializeField] private GameObject mainMenuPanel, player, entryCamera, videoPlayer, videoPlayer2;             //portal, 
        [SerializeField] private Camera mainCamera;

        //[Header("Test Variables")]
        //[SerializeField] private AnimationClip testClip;

        private void OnEnable()
        {
            localGameLogic.OnRestartClicked += ResetPlayerPosition;
        }

        private void OnDisable()
        {
            localGameLogic.OnRestartClicked -= ResetPlayerPosition;
        }

        private void Start()
        {
            //Debug.Log($"Starting Entry Script");
            //Debug.Log($"Clip Length : {testClip.length}");

#if !SKIP_ENTRY
            //disabledObjects[1].SetActive(true);
            //disabledObjects[3].SetActive(false);
            //backgroundAnimator.Play("Entry", 0);
            //Invoke(nameof(EnablePortal), 9.5f);
            //Invoke(nameof(EnablePlayer), 10f);
            Invoke(nameof(SetEnvironment), 11.5f);
            //player.transform.position = new Vector2(-8.43f, -2.3f);
#else
            disabledObjects[3].SetActive(false);
            mainCamera.enabled = true;
            player.transform.position = new Vector2(-5.3f, -3.7f);
            player.SetActive(true);                                  //Enable For Actual Gameplay
            //localBG_Controller.enabled = true;            //Enable BackGround Controller Script         //Enable For Actual Gameplay
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            //backgroundAnimator.runtimeAnimatorController = backgroundAnimatorControllers[1];
            mainMenuPanel.SetActive(true);
#endif

            #region CheckAnimationClipLength;
            //AnimationClip[] clips = playerAnimator.runtimeAnimatorController.animationClips;
            //foreach (var cl in clips)
            //{
            //    Debug.Log($" Clip : {cl.name}, Length : {cl.length}");
            //}
            #endregion CheckAnimationClipLength;
        }

#if ENABLE_PORTAL
        private void EnablePortal()
        {
            portal.SetActive(true);

            //Disable Title/Black Card
            disabledObjects[0].SetActive(false);
            disabledObjects[1].SetActive(false);
            disabledObjects[2].SetActive(false);

            portal.SetActive(true);
            portalAnimator.Play("Entry", 0);
            StartCoroutine(DisableObjectsAfter(2.6f, 0));
        }
#endif

        private void EnablePlayer()
        {
            player.SetActive(true);
            playerAnimator.Play("Entry", 0);
            StartCoroutine(DisableObjectsAfter(1.75f, 1));              //<==========Entry should be over by this point
        }

        private void DisableMask()
        {
            disabledObjects[2].SetActive(false);
        }

        private void SetEnvironment()
        {
            mainCamera.enabled = true;
            //videoPlayer.SetActive(true);
            //videoPlayer2.SetActive(true);
            disabledObjects[3].SetActive(false);
            player.SetActive(true);
            StartCoroutine(DisableObjectsAfter(0f, 1));              //<==========Entry should be over by this point
        }

        private void ResetPlayerPosition(int dummyData)
        {
            _ = StartCoroutine(DisableObjectsAfter(0, 1));
        }

        private IEnumerator DisableObjectsAfter(float seconds, int objectIndex)
        {
            switch (objectIndex)
            {
                case 0:
                    {
                        yield return new WaitForSeconds(seconds);
                        disabledObjects[3].SetActive(false);

                        break;
                    }

                case 1:
                    {
                        yield return new WaitForSeconds(seconds);
                        player.transform.position = new Vector2(-5.58f, -3.7f);
                        player.GetComponent<PlayerController>().enabled = true;
                        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                        //backgroundAnimator.runtimeAnimatorController = backgroundAnimatorControllers[1];
                        mainMenuPanel.SetActive(true);

                        //backgroundAnimator.Play("NightAnim", 0);                        //If left to nothing, cannot manipulate transform as the animator would be on
                        //backgroundAnimator.enabled = false;

                        //localBG_Controller.enabled = true;                              //Enable BackGround Controller Script
                        //localGameLogic.OnMainGameplayStarted?.Invoke();                 //Invoke Action of Main Gameplay has started
                        //Debug.Log($"Status : {localBG_Controller.enabled}");

                        break;
                    }

                default:
                    {
                        yield return null;
                        Debug.LogError($"Wrong Case Selected under EntryScript for DisableObjects: {objectIndex}");

                        break;
                    }
            }
        }
    }
}