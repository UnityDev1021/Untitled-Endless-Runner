//#define TEST_MODE

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
        private sbyte currentHeart = 4;                      //By defautl will be 4. Will increase as time goes by or the player collects more hearts.
        private bool halfHeart;

        [Header("UI")]
        [SerializeField] private Sprite[] heartSprites;
        [SerializeField] private GameObject heartContainer, buyHeartsPanel, airDashBt, jumpBt, mainMenuPanel;
        [SerializeField] private Image armorTimer, score2xTimer;
        [SerializeField] private GameObject[] heartsDisabledPanel;
        [SerializeField] private Button[] buyHeartsBt;

        [Header("UI Text")]
        [SerializeField] private TMP_Text finalScoreTxt;
        [SerializeField] private TMP_Text totalCoinsTxt, highScoreTxt, coinsBalanceTxt, diamondsTxt, coinsCollectedTxt, diamondsBalTxt;

        [Header ("Prefabs List")]
        [SerializeField] private GameObject heartPrefab;

        [Header ("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Animation")]
        [SerializeField] private Animator canvasAnimator;

        [Space]
        [SerializeField] private GameObject FadeScreen;

        [Header("Power-Up Durations")]
        [SerializeField] private float armorDurationMultiplier = 0.08f;
        [SerializeField] private float scoreDurationMultiplier = 0.08f, dashDurationMultiplier = 0.08f, hjDurationMultiplier = 0.08f;
        private Coroutine armourCoroutine, scoreCoroutine, dashCoroutine, hjCoroutine;

        private void OnEnable()
        {
            localGameLogic.OnObstacleDetected += UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver += DisplayBuyHeartsPanel;
            localGameLogic.OnPlayerCaptured += EmptyHearts;
            localGameLogic.OnRestartFinished += CallFadeOut;
            localGameLogic.OnRestartFinished += FillHearts;
            localGameLogic.OnRestartFinished += UpdateDiamondsAmount;
            localGameLogic.OnGameOver += UpdateFinalScore;
            localGameLogic.OnPowerUpCollected += UpdatePowerUpUI;
            localGameLogic.OnPowersBought += UpdateCoins;
            localGameLogic.OnAdsRewarded += UpdateDiamondsAmount;

#if TEST_MODE
            localGameLogic.FillUpPlayerHealth += RestorePlayerHealth;
#endif
        }
        private void OnDisable()
        {
            localGameLogic.OnObstacleDetected -= UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver -= DisplayBuyHeartsPanel;
            localGameLogic.OnPlayerCaptured -= EmptyHearts;
            localGameLogic.OnRestartFinished -= CallFadeOut;
            localGameLogic.OnRestartFinished -= FillHearts;
            localGameLogic.OnRestartFinished -= UpdateDiamondsAmount;
            localGameLogic.OnGameOver -= UpdateFinalScore;
            localGameLogic.OnPowerUpCollected -= UpdatePowerUpUI;
            localGameLogic.OnPowersBought -= UpdateCoins;
            localGameLogic.OnAdsRewarded -= UpdateDiamondsAmount;

#if TEST_MODE
            localGameLogic.FillUpPlayerHealth -= RestorePlayerHealth;
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
            currentHeart = (sbyte)(localGameLogic.totalHearts - 1);
#if TEST_MODE
            PlayerPrefs.SetInt("COIN_AMOUNT", 200);
#endif
            coinsBalanceTxt.text = PlayerPrefs.GetInt("COIN_AMOUNT", 0).ToString();
            diamondsTxt.text = PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0).ToString();
        }

        private void UpdateHeartUI(ObstacleStat obstacleStat)
        {
            //Debug.Log($"Calling UPgrade");
            switch (obstacleStat.type)
            {
                case ObstacleType.Boost:
                case ObstacleType.Normal:
                case ObstacleType.Normal_Attack:
                    break;

                case ObstacleType.Attack:
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

                        break;
                    }
                case ObstacleType.Power_Up:
                    {
                        //Debug.Log($"Calling Heart PowerUp");

                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.Heart:
                                {
                                    if (currentHeart == (byte)(heartContainer.transform.childCount - 1))                  //if it is less than or equal to 3, as the total hearts is 4
                                    {
                                        var heartAdded = Instantiate(heartPrefab, heartContainer.transform);
                                        heartAdded.transform.SetSiblingIndex(currentHeart);
                                        heartAdded.name = "Hearts_" + (currentHeart+1);
                                        currentHeart++;
                                    }
                                    else
                                    {                                        
                                        //if half-heart, then fill the next heart with half heart, and fill the current heart with full heart
                                        if (halfHeart)
                                        {
                                            heartContainer.transform.GetChild(currentHeart).GetComponent<Image>().sprite = heartSprites[0];
                                            heartContainer.transform.GetChild(currentHeart + 1).GetComponent<Image>().sprite = heartSprites[1];
                                        }
                                        //if the next heart is empty, then fill the next heart with full heart
                                        else
                                        {
                                            //Debug.Log($"Temp Heart Count : {currentHeart}");
                                            //byte tempHeartCount = (currentHeart == 0) ? (byte) 0 : (byte) (currentHeart + 1);             //Consider Reset
                                            //Debug.Log($"Temp Heart Count : {tempHeartCount}");

                                            heartContainer.transform.GetChild(currentHeart + 1).GetComponent<Image>().sprite = heartSprites[0];
                                        }

                                        //currentHeart = (currentHeart == 0) ? (sbyte) 0 : currentHeart++;             //Consider Reset
                                        currentHeart++;
                                    }
                                    //Debug.Log($"Current Heart : {currentHeart}");

                                    break;
                                }

                            default:
                                {
                                    Debug.LogError($"Osbtacle Tag Not Found : {obstacleStat.tag}");
                                    break;
                                }
                        }

                        break;
                    }

                default:
                    {
                        Debug.LogError($"Osbtacle Type Not Found : {obstacleStat.type}");
                        break;
                    }
            }
        }

        #region OldCode
        //Moved to PlayController
        /*private IEnumerator DisplayHurtPanel()
        {
            float time = 0, startVal = 0f, endVal = 0.45f, timeSpeedMultiplier = 2f;
             byte executedTime = 0;

             while (true)
             {
                 time += timeSpeedMultiplier * Time.deltaTime;

                 if (time >= 1f)
                 {
                     float temp = startVal;
                     startVal = endVal;
                     endVal = temp;
                     time = 0f;

                     executedTime++;
                     Debug.Log($"Executed Time : {executedTime}");
                     if (executedTime == 2)                  //Break when returning to original value of 0
                     {
                         Debug.Log($"Breaking : {executedTime}");
                         break;
                     }
                 }

                 playerHurtPanel.color = new Color(1f, 0f, 0f, Mathf.Lerp(startVal, endVal, time));

                 yield return null;
             }
        }*/
        #endregion OldCode

        //On the BuyHeartsBt under the BuyHeartsPanel
        public void ResetCurrentHeart()
        {
            currentHeart = 0;
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
            currentHeart = (sbyte)(localGameLogic.totalHearts - 1);

            for (int i = 0; i < heartContainer.transform.childCount; i++)
            {
                heartContainer.transform.GetChild(i).GetComponent<Image>().sprite = heartSprites[0];
            }
        }

        private void DisplayBuyHeartsPanel()
        {
            foreach(var disabledPanel in heartsDisabledPanel)            
                disabledPanel.SetActive(true);            

            buyHeartsPanel.SetActive(true);
            diamondsBalTxt.text = PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0).ToString();

            if (PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0) >= 3)
            {
                buyHeartsBt[0].enabled = true;
                heartsDisabledPanel[0].SetActive(false);
            }
            if (PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0) >= 5)
            {
                buyHeartsBt[1].enabled = true;
                heartsDisabledPanel[1].SetActive(false);
            }
            if (PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0) >= 8)
            {
                buyHeartsBt[2].enabled = true;
                heartsDisabledPanel[2].SetActive(false);
            }

            //Debug.Log($"Game Over Panel Active : {gameOverPanel.activeSelf}");
        }

        //On BuyHearts button under BuyHearts Panel
        public void BuyHearts(int heartCost)
        {
            int diamondsAmount = PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0);
            diamondsAmount -= heartCost;
            PlayerPrefs.SetInt("DIAMONDS_AMOUNT", diamondsAmount);
            localGameLogic.OnGameplayContinued?.Invoke();           //Alive Player            
        }

        private void UpdateDiamondsAmount()
        {
            diamondsTxt.text = PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0).ToString();
            diamondsBalTxt.text = PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0).ToString();
            totalCoinsTxt.text = "0";
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

            coinsCollectedTxt.text = totalCoinsTxt.text;
            coinsBalanceTxt.text = PlayerPrefs.GetInt("COIN_AMOUNT", 0).ToString();
            //Debug.Log("Coins Amount After : " + PlayerPrefs.GetInt("COIN_AMOUNT", -1));
        }

        private void UpdateCoins()
        {
            coinsBalanceTxt.text = GameManager.instance.coinsBalance.ToString();
        }

        private void UpdatePowerUpUI(ObstacleTag detectedTag, int amount)
        {
            switch (detectedTag)
            {
                //Do Nothing
                case ObstacleTag.SpeedBoost:
                    break;

                case ObstacleTag.Coin:
                    {
                        if (amount > 1)
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

                case ObstacleTag.HigherJump:
                    {
                        if (amount == 1)
                            ShowHigherJumpTimer();

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
            armourCoroutine = StartCoroutine(StartTimer(ObstacleTag.Shield));
        }

        private void ShowScore2xTimer()
        {
            score2xTimer.gameObject.SetActive(true);

            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
            }
            scoreCoroutine = StartCoroutine(StartTimer(ObstacleTag.Score2x));
        }

        private void ShowAirDashTimer()
        {
            airDashBt.SetActive(true);

            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }
            dashCoroutine = StartCoroutine(StartTimer(ObstacleTag.Dash));
        }

        private void ShowHigherJumpTimer()
        {
            jumpBt.transform.GetChild(0).gameObject.SetActive(false);
            jumpBt.transform.GetChild(1).gameObject.SetActive(true);

            if (hjCoroutine != null)
            {
                StopCoroutine(hjCoroutine);
            }
            hjCoroutine = StartCoroutine(StartTimer(ObstacleTag.HigherJump));
        }

        private IEnumerator StartTimer(ObstacleTag tagDetected)           //0 is for coin
        {
            Debug.Log($"Starting TImer");

            float tempTime = 0f;

            switch (tagDetected)
            {
                //Do Nothing
                case ObstacleTag.Coin:
                    break;

                case ObstacleTag.Shield:
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

                case ObstacleTag.Score2x:
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

                case ObstacleTag.Dash:
                    {
                        while (true)
                        {
                            tempTime += dashDurationMultiplier * Time.deltaTime;                              //0.2f is too fast

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

                case ObstacleTag.HigherJump:
                    {
                        Image higherJumpSprite = jumpBt.transform.GetChild(1).GetComponent<Image>();
                        while (true)
                        {
                            tempTime += hjDurationMultiplier * Time.deltaTime;                              //0.2f is too fast

                            if (tempTime >= 1)
                            {
                                localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.HigherJump, 0);
                                jumpBt.transform.GetChild(0).gameObject.SetActive(true);
                                jumpBt.transform.GetChild(1).gameObject.SetActive(false);

                                break;
                            }

                            higherJumpSprite.fillAmount = Mathf.Lerp(1, 0, tempTime);
                            //Debug.Log($"Temp Time : {tempTime}, Dash FIll Amount : {airDashBt.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount}");

                            yield return null;
                        }

                        break;
                    }

                default:
                    {
                        Debug.LogError($"PowerUP Index not fuond : {tagDetected}");
                        break;
                    }
            }
        }

        private void UpdateTotalCoins(ref int totalCoins)
        {
            //Debug.Log("Updating Coins");
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
            mainMenuPanel.SetActive(true);
        }
    }
}