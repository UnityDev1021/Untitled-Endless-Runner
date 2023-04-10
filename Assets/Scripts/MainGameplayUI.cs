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

        [Header ("Prefabs List")]
        [SerializeField] private GameObject heartPrefab;

        [Header ("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Animation")]
        [SerializeField] private Animator canvasAnimator;

        [Space]
        [SerializeField] private GameObject FadeScreen;

        private void OnEnable()
        {
            localGameLogic.OnObstacleDetected += UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver += DisplayEndGameScreen;
            localGameLogic.OnPlayerCaptured += EmptyHearts;
            localGameLogic.OnRestartFinished += CallFadeOut;
            localGameLogic.OnRestartFinished += FillHearts;
            localGameLogic.OnGameOver += UpdateFinalScore;
            localGameLogic.OnCoinCollected += UpdateTotalCoins;
        }
        private void OnDisable()
        {
            localGameLogic.OnObstacleDetected -= UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver -= DisplayEndGameScreen;
            localGameLogic.OnPlayerCaptured -= EmptyHearts;
            localGameLogic.OnRestartFinished -= CallFadeOut;
            localGameLogic.OnRestartFinished -= FillHearts;
            localGameLogic.OnGameOver -= UpdateFinalScore;
            localGameLogic.OnCoinCollected -= UpdateTotalCoins;
        }

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
            Debug.Log($"Game Over Panel Active : {gameOverPanel.activeSelf}");
        }

        private void UpdateFinalScore(int finalScore)
        {
            finalScoreTxt.text = finalScore.ToString();
        }

        private void UpdateTotalCoins(int totalCoins)
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