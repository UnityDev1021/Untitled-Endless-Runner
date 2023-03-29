using System;
using UnityEngine;
using UnityEngine.UI;

namespace Untitled_Endless_Runner
{
    public class MainGameplayUI : MonoBehaviour
    {
        [Header ("Heart Logic")]
        private byte currentHeart;
        private bool halfHeart; 

        [Header("UI")]
        [SerializeField] private GameObject heartContainer;
        [SerializeField] private Sprite[] heartSprites;

        [Header ("Prefabs List")]
        [SerializeField] private GameObject heartPrefab;

        [Header ("Local References")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic.OnObstacleDetected += UpdateHealth;
            localGameLogic.OnPlayerHealthOver += DisplayEndGameScreen;
        }
        private void OnDisable()
        {
            localGameLogic.OnObstacleDetected -= UpdateHealth;
            localGameLogic.OnPlayerHealthOver -= DisplayEndGameScreen;
        }
        private void Start()
        {
            currentHeart = (byte)(heartContainer.transform.childCount - 1);
        }

        private void UpdateHealth(ObstacleStat obstacleStat, float dummyData2)
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

        }
    }
}