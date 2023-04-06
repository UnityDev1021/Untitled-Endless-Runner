using System;
using UnityEditor.Animations;
using UnityEngine;

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
        public Action<int> OnRestartClicked;

        [Header("Local Refernece Scripts")]
        [SerializeField] private BackGroundController localBG_Controller;
        [SerializeField] private ObstacleSpawnerTest localObstacleSpawner;

        [Header("Animator Refernece")]
        [SerializeField] private Animator backgroundAnimator;
        [SerializeField] private Animator playerAnimator, canvasAnimator;

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
            localBG_Controller.enabled = true;
            localObstacleSpawner.enabled = true;
            //backgroundAnimator.keepAnimatorStateOnDisable = true;
            backgroundAnimator.Play("Day_Night_Cycle", 0, 0f);

            //playerAnimator.SetBool("RUN", true);
            playerAnimator.Play("Idle_Run", 0, 0f);
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }

        private void ToggleGameStatus(bool toggleValue)
        {
            if (toggleValue)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }

        private void RestartGame(int dummyData)
        {
            ToggleGameStatus(false);

            localBG_Controller.enabled = false;
            localObstacleSpawner.enabled = false;

            //backgroundAnimator.runtimeAnimatorController = backgroundAnimatorControllers[0];                  //Turn off Animator all together
            backgroundAnimator.Play("Nothing", 0, 1f);

            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            mainCamera.transform.position = new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z);
            OnRestartFinished?.Invoke();
        }

        private void GoHome()
        {

        }

        private void EndGamePlay()
        {

        }

        //On Main Menu, under the Exit button
        public void ExitGame()
        {
           Application.Quit();
        }
    }
}