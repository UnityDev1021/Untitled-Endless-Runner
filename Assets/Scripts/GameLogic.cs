using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class GameLogic : MonoBehaviour
    {
        private byte _totalHearts = 4;
        public int totalHearts { get => _totalHearts; }

        [SerializeField] private GameObject player;

        public Action<ObstacleStat> OnObstacleDetected;
        public Action OnPlayerHealthOver, OnPlayerCaptured, OnGamePlayStarted, OnMainGameplayStarted, 
            OnResumeClicked, OnRestartClicked, OnHomeClicked;
        public Action<bool> OnPause_ResumeClicked;

        [Header("Local Refernece Scripts")]
        [SerializeField] private BackGroundController localBG_Controller;
        [SerializeField] private ObstacleSpawnerTest localObstacleSpawner;

        [Header("Animator Refernece")]
        [SerializeField] private Animator backgroundAnimator;
        [SerializeField] private AnimatorController[] backgroundAnimatorControllers;

        private void OnEnable()
        {
            OnMainGameplayStarted += EnableObjects;
            OnPlayerHealthOver += EndGame;
            OnPause_ResumeClicked += ToggleGameStatus;
            OnRestartClicked += RestartGame;
            OnHomeClicked += GoHome;
        }

        private void OnDisable()
        {
            OnMainGameplayStarted -= EnableObjects;
            OnPlayerHealthOver -= EndGame;
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
            //backgroundAnimator.Play("Day_Night_Cycle", 0, 0f);
            backgroundAnimator.runtimeAnimatorController = backgroundAnimatorControllers[1];
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

        private void RestartGame()
        {

        }

        private void GoHome()
        {

        }

        private void EndGame()
        {

        }
    }
}