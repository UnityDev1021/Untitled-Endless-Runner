#define TEST_MODE

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Untitled_Endless_Runner
{
    public class MainGameplayUI : MonoBehaviour
    {
        [Header ("Heart Logic")]
        private byte currentHeart = 4;                      //By defautl will be 4. Will increase as time goes by or the player collects more hearts.
        private bool halfHeart;

        [Header("UI")]
        [SerializeField] private Sprite[] heartSprites;
        [SerializeField] private GameObject heartContainer, gameOverPanel;
        [SerializeField] private TMP_Text finalScoreTxt, totalCoinsTxt;
        [SerializeField] private Image armorTimer;

        [Header ("Prefabs List")]
        [SerializeField] private GameObject heartPrefab;

        [Header ("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Animation")]
        [SerializeField] private Animator canvasAnimator;

        [Space]
        [SerializeField] private GameObject FadeScreen;
        private float armorDurationMultiplier = 0.2f;

        private void OnEnable()
        {
            localGameLogic.OnObstacleDetected += UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver += DisplayEndGameScreen;
            localGameLogic.OnPlayerCaptured += EmptyHearts;
            localGameLogic.OnRestartFinished += CallFadeOut;
            localGameLogic.OnRestartFinished += FillHearts;
            localGameLogic.OnGameOver += UpdateFinalScore;
            localGameLogic.OnPowerUpCollected += UpdatePowerUpUI;

#if TEST_MODE
            //localGameLogic.FillUpPlayerHealth += RestorePlayerHealth;
#endif
        }
        private void OnDisable()
        {
            localGameLogic.OnObstacleDetected -= UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver -= DisplayEndGameScreen;
            localGameLogic.OnPlayerCaptured -= EmptyHearts;
            localGameLogic.OnRestartFinished -= CallFadeOut;
            localGameLogic.OnRestartFinished -= FillHearts;
            localGameLogic.OnGameOver -= UpdateFinalScore;
            localGameLogic.OnPowerUpCollected -= UpdatePowerUpUI;

#if TEST_MODE
            //localGameLogic.FillUpPlayerHealth -= RestorePlayerHealth;
#endif
        }

#if TEST_MODE
        //On the Restore button, under the Test Canvas
        public void RestorePlayerHealth()
        {
            Debug.Log($"Restoring HEalth");
            localGameLogic.FillUpPlayerHealth?.Invoke();
            Start();
            FillHearts();
        }
#endif

        private void Start()
        {
            currentHeart = (byte)(localGameLogic.totalHearts - 1);
        }

        private void UpdateHeartUI(ObstacleStat obstacleStat)
        {
            if (obstacleStat.type.Equals(ObstacleType.Attack))
            {
                if (!halfHeart)
                {
                    heartContainer.transform.GetChild(currentHeart).GetComponent<Image>().sprite = heartSprites[1];
                    halfHeart = true;
                }
                else
                {
                    heartContainer.transform.GetChild(currentHeart).GetComponent<Image>().sprite = heartSprites[2];
                    currentHeart--;
                    halfHeart = false;
                }
            }
        }

        private void EmptyHearts()
        {
            for (int i = 0; i < heartContainer.transform.childCount ; i++)
            {
                heartContainer.transform.GetChild(i).GetComponent<Image>().sprite = heartSprites[2];
            }
        }

        private void FillHearts()
        {
            currentHeart = (byte)(localGameLogic.totalHearts - 1);

            for (int i = 0; i < heartContainer.transform.childCount; i++)
            {
                heartContainer.transform.GetChild(i).GetComponent<Image>().sprite = heartSprites[0];
            }
        }

        private void DisplayEndGameScreen()
        {
            gameOverPanel.SetActive(true);
            //Debug.Log($"Game Over Panel Active : {gameOverPanel.activeSelf}");
        }

        private void UpdateFinalScore(int finalScore)
        {
            finalScoreTxt.text = finalScore.ToString();
        }

        private void UpdatePowerUpUI(ObstacleTag detectedTag, int amount)
        {
            switch (detectedTag)
            {
                case ObstacleTag.Coin:
                    {
                        UpdateTotalCoins(ref amount);
                        break;
                    }

                case ObstacleTag.Shield:
                    {
                        ShowArmorTimer();
                        break;
                    }

                default:
                    {
                        Debug.LogError($"Wrong Tag Detected under UpdatePowerUI : {detectedTag.ToString()}");
                        break;
                    }
            }
        }

        private void ShowArmorTimer()
        {
            armorTimer.gameObject.SetActive(true);
            _ = StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer()
        {
            float tempTime = 0f;
            while (true)
            {
                tempTime += armorDurationMultiplier * Time.deltaTime;                              //0.2f is too fast

                if (tempTime >= 1)                
                    break;

                armorTimer.fillAmount = Mathf.Lerp(1, 0, tempTime);
                Debug.Log($"Temp Time : {tempTime}, armor FIll Amount : {armorTimer.fillAmount}");

                yield return null;
            }
        }

        private void UpdateTotalCoins(ref int totalCoins)
        {
            Debug.Log("Updating Coins");
            totalCoinsTxt.text = totalCoins.ToString();
        }

        //On Gameplay UI, under the pause/resume button
        public void Pause_ResumeClicked(bool toggleValue)
        {
            localGameLogic.OnPause_ResumeClicked?.Invoke(toggleValue);
        }

        //On Pause Panel, under the restart button
        public void RestartClicked()
        {
            FadeScreen.SetActive(true);
            canvasAnimator.Play("FadeIn", 0, 0f);
            localGameLogic.OnRestartClicked?.Invoke(0);
        }

        private void CallFadeOut()
        {
            _ = StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(1.1f);
            canvasAnimator.Play("FadeOut", 0, 0f);

            yield return new WaitForSeconds(1.1f);
            FadeScreen.SetActive(false);
        }

        //On Pause Panel, under the restart button
        public void HomeClicked()
        {
            localGameLogic.OnHomeClicked?.Invoke();
        }
    }
}