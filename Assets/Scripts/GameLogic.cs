//#define TEST_MODE

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
        [SerializeField] private GameObject[] musicImgStatus, soundImgStatus;

        public Action<ObstacleStat> OnObstacleDetected;
        public Action OnPlayerHealthOver, OnPlayerCaptured, OnResumeClicked, OnRestartFinished, OnPowersBought, 
            OnAdsRewarded, OnGameplayContinued;
        public Action<bool> OnPause_ResumeClicked;
        public Action<int> OnRestartClicked, OnGameOver;
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

        [Header("Power Ups Section")]
        [SerializeField] private byte[] powersCost;
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
         *  ExtraHearts - 5
         *******************************************************************************************/
        //On Debug Power buttons, under the Test Canvas
        public void StartUpPowers(int powerIndex)
        {
            //Debug.Log("Binary : " + Convert.ToString(startPowers, 2));
            //startPowers = 0;
            //Debug.Log($"coins Balance : {GameManager.instance.coinsBalance}, COIN_AMOUNT : {PlayerPrefs.GetInt("COIN_AMOUNT", -1)}");

            try
            {
                //Debug.Log($"powersIndex : {powerIndex}, condition 4 : {(powerIndex & (1 << 4))}");
                if (GameManager.instance.coinsBalance >= powersCost[powerIndex])             //Don't have enough coins to buy power-up
                {
                    GameManager.instance.coinsBalance -= powersCost[powerIndex];

                    if (powerIndex == 5)                                                //Heart Power-Up
                        OnObstacleDetected?.Invoke(GameManager.instance.tagsToBeDetected[powerIndex]);
                    //else
                    //    OnPowerUpCollected?.Invoke(GameManager.instance.tagsToBeDetected[powerIndex].tag, 1);
                }
                else
                    return;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error Found under StartUpPowers in GameLogic : {e}");
            }

            startPowers |= (1 << powerIndex);
            PlayerPrefs.SetInt("COIN_AMOUNT", GameManager.instance.coinsBalance);
            OnPowersBought?.Invoke();

            Debug.Log($"Binary : {Convert.ToString(startPowers, 2)}, coins Balance : {GameManager.instance.coinsBalance}, COIN_AMOUNT : {PlayerPrefs.GetInt("COIN_AMOUNT", -1)}");
        }

        public void Start()
        {
            GameManager.instance.coinsBalance = PlayerPrefs.GetInt("COIN_AMOUNT", 0);
        }

        //On the Start Panel under the "Tap To Play" button
        public void StartGame()
        {
            disabledObjects[0].SetActive(true);
            GameManager.instance.gameStarted = true;

            //localBG_Controller.enabled = true;           //Replaced with buttons
            //localObstacleSpawner.enabled = true;           //Replaced with buttons

            //backgroundAnimator.keepAnimatorStateOnDisable = true;
            backgroundAnimator.enabled = true;                                //Keeping BG static
            backgroundAnimator.Play("Day_Night_Cycle2", 0, 0f);                                //Keeping BG static

            //backgroundAnimator.enabled = true;                                //Keeping BG static
            //backgroundAnimator.Play("DayOnly", 0, 0f);                                //Keeping BG static

            //playerAnimator.SetBool("RUN", true);
            playerAnimator.Play("Idle_Run", 0, 0f);
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            InvokeStartPowers();
        }

        private void InvokeStartPowers()
        {
            for (int i = 0; i < 5; i++)
            {
                if ((startPowers & (1 << i)) != 0)               //The "i" contains the Power Index which needs to be activated.
                {
                    OnPowerUpCollected?.Invoke(GameManager.instance.tagsToBeDetected[i].tag, 1);
                }
                //Debug.Log($"Power Index : {Convert.ToString(powerIndex, 2)}, i : {i} , condition : {(powerIndex & (1 << i))}");
            }
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

            //GameManager.instance.gameStarted = false;
            //localObstacleSpawner.enabled = false;           //Replaced with buttons
        }

        //On the Sound Effect button, under the Pause Panel
        public void ToggleSE()
        {
            toggleSE = !toggleSE;

            if (toggleSE)
            {
                soundImgStatus[0].SetActive(true);
                soundImgStatus[1].SetActive(false);
                soundImgStatus[2].SetActive(true);
                soundImgStatus[3].SetActive(false);
            }
            else
            {
                soundImgStatus[0].SetActive(false);
                soundImgStatus[1].SetActive(true);
                soundImgStatus[2].SetActive(false);
                soundImgStatus[3].SetActive(true);
            }

            audioMixers[0].audioMixer.SetFloat("VolumeSE", !toggleSE ? -80f : 0f);
        }

        //On the Music button, under the Pause Panel
        public void ToggleBGM()
        {
            toggleMusic = !toggleMusic;

            if (toggleMusic)
            {
                musicImgStatus[0].SetActive(true);
                musicImgStatus[1].SetActive(false);
                musicImgStatus[2].SetActive(true);
                musicImgStatus[3].SetActive(false);
            }
            else
            {
                musicImgStatus[0].SetActive(false);
                musicImgStatus[1].SetActive(true);
                musicImgStatus[2].SetActive(false);
                musicImgStatus[3].SetActive(true);
            }
            audioMixers[1].audioMixer.SetFloat("VolumeBGM", !toggleMusic ? -80f : -10f);
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