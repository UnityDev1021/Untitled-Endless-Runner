#define TEST_MODE

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
        public Action OnPlayerHealthOver, OnPlayerCaptured, OnResumeClicked, OnRestartFinished;
        public Action<bool> OnPause_ResumeClicked;
        public Action<int> OnRestartClicked, OnGameOver, OnMainGameplayStarted;
        public Action<ObstacleTag, int> OnPowerUpCollected;

        [Header("Player Actions")]
        public Action<PlayerAction, byte> OnPlayerAction;

#if TEST_MODE
        [Header("Test Actions")]
        public Action FillUpPlayerHealth;
#endif

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
        private int startPowers;
        //public bool gameplayBegan;

        private void OnEnable()
        {
            OnPlayerHealthOver += EndGamePlay;
            OnPause_ResumeClicked += ToggleGameStatus;
            OnRestartClicked += RestartGame;

#if TEST_MODE
            FillUpPlayerHealth += RestorePlayerHealth;
#endif
        }

        private void OnDisable()
        {
            OnPlayerHealthOver -= EndGamePlay;
            OnPause_ResumeClicked -= ToggleGameStatus;
            OnRestartClicked -= RestartGame;

#if TEST_MODE
            FillUpPlayerHealth -= RestorePlayerHealth;
#endif
        }


#if TEST_MODE
        private void RestorePlayerHealth() { }
#endif

        /*******************************************************************************************
         *  Shield - 0
         *  Score2x - 1
         *  AirDash - 2
         *  HigherJump - 3
         *  SpeedBoost - 4
         *******************************************************************************************/
        //On Debug Power buttons, under the Test Canvas
        public void StartUpPowers(int powerIndex)
        {
            //Debug.Log("Binary : " + Convert.ToString(startPowers, 2));
            //startPowers = 0;

            if ((powerIndex & (1 << 0)) != 0)
            {
                Debug.Log($"condition 0 : { (powerIndex & (1 << 0))}");
                if (GameManager.instance.coinsBalance >= 10)             //Don't have enough coins to buy power-up
                    GameManager.instance.coinsBalance -= 10;
                else
                    return;
            }
            else if ((powerIndex & (1 << 1)) != 0)
            {
                Debug.Log($"condition 1 : {(powerIndex & (1 << 1))}");
                if (GameManager.instance.coinsBalance >= 5)             //Don't have enough coins to buy power-up
                    GameManager.instance.coinsBalance -= 5;
                else
                    return;
            }
            else if ((powerIndex & (1 << 2)) != 0)
            {
                Debug.Log($"condition 2 : {(powerIndex & (1 << 2))}");
                if (GameManager.instance.coinsBalance >= 7)             //Don't have enough coins to buy power-up
                    GameManager.instance.coinsBalance -= 7;
                else
                    return;
            }
            else if ((powerIndex & (1 << 3)) != 0)
            {
                Debug.Log($"condition 3 : {(powerIndex & (1 << 3))}");
                if (GameManager.instance.coinsBalance >= 2)             //Don't have enough coins to buy power-up
                    GameManager.instance.coinsBalance -= 2;
                else
                    return;
            }
            else if ((powerIndex & (1 << 4)) != 0)
            {
                Debug.Log($"condition 4 : {(powerIndex & (1 << 4))}");
                if (GameManager.instance.coinsBalance >= 8)             //Don't have enough coins to buy power-up
                    GameManager.instance.coinsBalance -= 8;
                else
                    return;
            }

            startPowers |= (1 << powerIndex);

            Debug.Log($"Binary : {Convert.ToString(startPowers, 2)}");
        }

        //On the Start Panel under the "Tap To Play" button
        public void StartGame()
        {
            disabledObjects[0].SetActive(true);
            GameManager.instance.gameStarted = true;

            //localBG_Controller.enabled = true;           //Replaced with buttons
            //localObstacleSpawner.enabled = true;           //Replaced with buttons

            //backgroundAnimator.keepAnimatorStateOnDisable = true;
            backgroundAnimator.enabled = true;
            backgroundAnimator.Play("Day_Night_Cycle2", 0, 0f);

            //playerAnimator.SetBool("RUN", true);
            playerAnimator.Play("Idle_Run", 0, 0f);
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            OnMainGameplayStarted?.Invoke(startPowers);
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
            startPowers = 0;

            //localBG_Controller.enabled = false;           //Replaced with buttons

            //backgroundAnimator.runtimeAnimatorController = backgroundAnimatorControllers[0];                  //Turn off Animator all together
            backgroundAnimator.Play("Nothing", 0, 1f);

            //player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            mainCamera.transform.position = new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z);
            OnRestartFinished?.Invoke();

            GameManager.instance.gameStarted = false;
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