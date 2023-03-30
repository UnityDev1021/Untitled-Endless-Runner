using System;
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

        [Header ("Prefabs List")]
        [SerializeField] private GameObject heartPrefab;

        [Header ("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic.OnObstacleDetected += UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver += DisplayEndGameScreen;
        }
        private void OnDisable()
        {
            localGameLogic.OnObstacleDetected -= UpdateHeartUI;
            localGameLogic.OnPlayerHealthOver -= DisplayEndGameScreen;
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

        private void DisplayEndGameScreen()
        {
            gameOverPanel.SetActive(true);
        }
    }
}