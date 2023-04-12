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
        [SerializeField] private GameObject heartContainer, gameOverPanel, airDashBt;
        [SerializeField] private TMP_Text finalScoreTxt, totalCoinsTxt, highScoreTxt;
        [SerializeField] private Image armorTimer, score2xTimer;

        [Header ("Prefabs List")]
        [SerializeField] private GameObject heartPrefab;

        [Header ("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Animation")]
        [SerializeField] private Animator canvasAnimator;

        [Space]
        [SerializeField] private GameObject FadeScreen;
        [SerializeField] private float armorDurationMultiplier = 0.2f, scoreDurationMultiplier = 0.2f;
        private Coroutine armourCoroutine, scoreCoroutine, airDashCoroutine;

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

            //=======================> Check High Score <=======================
            PlayerData playerData = SaveSystem.LoadHighScore();
            if (playerData != null)
            {
                if (finalScore > playerData.score)
                {
                    highScoreTxt.text = finalScore.ToString();
                    playerData.score = finalScore;
                    SaveSystem.SaveHighScore(playerData);
                }
                else
                {
                    highScoreTxt.text = playerData.score.ToString();
                }
            }       
            else                            //For first time saving the High Score
            {
                highScoreTxt.text = finalScore.ToString();
                playerData = new PlayerData(finalScore);
                SaveSystem.SaveHighScore(playerData);
            }
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
                        if (amount == 1)
                            ShowArmorTimer();

                        break;
                    }

                case ObstacleTag.Score2x:
                    {
                        if (amount == 1)
                            ShowScore2xTimer();

                        break;
                    }

                case ObstacleTag.Dash:
                    {
                        if (amount == 1)
                            ShowAirDashTimer();

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

            if (armourCoroutine != null)
            {
                StopCoroutine(armourCoroutine);
            }
            armourCoroutine = StartCoroutine(StartTimer(1));
        }

        private void ShowScore2xTimer()
        {
            score2xTimer.gameObject.SetActive(true);

            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
            }
            scoreCoroutine = StartCoroutine(StartTimer(2));
        }

        private void ShowAirDashTimer()
        {
            airDashBt.SetActive(true);

            if (airDashCoroutine != null)
            {
                StopCoroutine(airDashCoroutine);
            }
            airDashCoroutine = StartCoroutine(StartTimer(3));
        }

        private IEnumerator StartTimer(byte powerUpIndex)           //0 is for coin
        {
            Debug.Log($"Starting TImer");

            float tempTime = 0f;

            switch (powerUpIndex)
            {
                //Do Nothing
                case 0:
                    break;

                case 1:
                    {
                        while (true)
                        {
                            tempTime += armorDurationMultiplier * Time.deltaTime;                              //0.2f is too fast

                            if (tempTime >= 1)
                            {
                                localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Shield, 0);
                                armorTimer.gameObject.SetActive(false);
                                break;
                            }

                            armorTimer.fillAmount = Mathf.Lerp(1, 0, tempTime);
                            //Debug.Log($"Temp Time : {tempTime}, armor FIll Amount : {armorTimer.fillAmount}");

                            yield return null;
                        }

                        break;
                    }

                case 2:
                    {
                        while (true)
                        {
                            tempTime += scoreDurationMultiplier * Time.deltaTime;                              //0.2f is too fast

                            if (tempTime >= 1)
                            {
                                localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Score2x, 0);
                                score2xTimer.gameObject.SetActive(false);
                                break;
                            }

                            score2xTimer.fillAmount = Mathf.Lerp(1, 0, tempTime);
                            //Debug.Log($"Temp Time : {tempTime}, armor FIll Amount : {armorTimer.fillAmount}");

                            yield return null;
                        }

                        break;
                    }

                case 3:
                    {
                        while (true)
                        {
                            tempTime += scoreDurationMultiplier * Time.deltaTime;                              //0.2f is too fast

                            if (tempTime >= 1)
                            {
                                localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Dash, 0);
                                airDashBt.SetActive(false);
                                break;
                            }

                            airDashBt.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(1, 0, tempTime);
                            //Debug.Log($"Temp Time : {tempTime}, Dash FIll Amount : {airDashBt.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount}");

                            yield return null;
                        }

                        break;
                    }

                default:
                    {
                        Debug.LogError($"PowerUP Index not fuond : {powerUpIndex}");
                        break;
                    }
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