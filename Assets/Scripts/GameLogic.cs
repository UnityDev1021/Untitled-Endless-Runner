using System;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Audio;

namespace Untitled_Endless_Runner
{
    public class GameLogic : MonoBehaviour
    {
        private byte _totalHearts = 4;
        public int totalHearts { get => _totalHearts; }

        [SerializeField] private GameObject player, mainCamera;

        public Action<ObstacleStat> OnObstacleDetected;
        public Action OnPlayerHealthOver, OnPlayerCaptured, OnGamePlayStarted, OnMainGameplayStarted, 
            OnResumeClicked, OnHomeClicked, OnPlayerSlide, OnRestartFinished;
        public Action<bool> OnPause_ResumeClicked;
        public Action<int> OnRestartClicked, OnGameOver, OnCoinCollected;

        [SerializeField] private GameObject[] disabledObjects;

        //[Header("Local Refernece Scripts")]
        //[SerializeField] private BackGroundController localBG_Controller;
        //[SerializeField] private ObstacleSpawnerTest localObstacleSpawner;

        [Header("Animator Refernece")]
        [SerializeField] private Animator backgroundAnimator;
        [SerializeField] private Animator playerAnimator;

        [Space]
        public AudioMixerGroup[] audioMixers;
        private bool toggleMusic = true, toggleSE = true, toggleVibrate = true;
        public bool gameplayBegan;

        private void OnEnable()
        {
            OnMainGameplayStarted += EnableObjects;
            OnPlayerHealthOver += EndGamePlay;
            OnPause_ResumeClicked += ToggleGameStatus;
            OnRestartClicked += RestartGame;
            OnHomeClicked += GoHome;
        }

        private void OnDisable()
        {
            OnMainGameplayStarted -= EnableObjects;
            OnPlayerHealthOver -= EndGamePlay;
            OnPause_ResumeClicked -= ToggleGameStatus;
            OnRestartClicked -= RestartGame;
            OnHomeClicked -= GoHome;
        }

        private void EnableObjects()
        {

        }

        //On the Start Panel under the "Tap To Play" button
        public void StartGame()
        {
            disabledObjects[0].SetActive(true);
            gameplayBegan = true;

            //localBG_Controller.enabled = true;           //Replaced with buttons
            //localObstacleSpawner.enabled = true;           //Replaced with buttons

            //backgroundAnimator.keepAnimatorStateOnDisable = true;
            backgroundAnimator.enabled = true;
            backgroundAnimator.Play("Day_Night_Cycle2", 0, 0f);

            //playerAnimator.SetBool("RUN", true);
            playerAnimator.Play("Idle_Run", 0, 0f);
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }

        private void ToggleGameStatus(bool toggleValue)
        {
            if (toggleValue)
                Time.timeScale = 1f;
            else
                Time.timeScale = 0f;
        }

        private void RestartGame(int dummyData)
        {
            ToggleGameStatus(false);

            //localBG_Controller.enabled = false;           //Replaced with buttons

            //backgroundAnimator.runtimeAnimatorController = backgroundAnimatorControllers[0];                  //Turn off Animator all together
            backgroundAnimator.Play("Nothing", 0, 1f);

            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            mainCamera.transform.position = new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z);
            OnRestartFinished?.Invoke();

            gameplayBegan = false;
            //localObstacleSpawner.enabled = false;           //Replaced with buttons
        }

        //On the Sound Effect button, under the Pause Panel
        public void ToggleSE()
        {
            toggleSE = !toggleSE;
            audioMixers[0].audioMixer.SetFloat("VolumeSE", !toggleSE ? -80f : 0f);
        }

        //On the Music button, under the Pause Panel
        public void ToggleBGM()
        {
            toggleMusic = !toggleMusic;
            audioMixers[1].audioMixer.SetFloat("VolumeBGM", !toggleMusic ? -80f : 0f);
        }

        public void ToggleVibrate()
        {
            toggleVibrate = !toggleVibrate;
            PlayerPrefs.SetInt("toggleVibrate", !toggleVibrate ? 0 : 1);
        }

        private void GoHome()
        {

        }

        private void EndGamePlay()
        {
            disabledObjects[0].SetActive(false);
        }

        //On Main Menu, under the Exit button
        public void ExitGame()
        {
           Application.Quit();
        }
    }
}