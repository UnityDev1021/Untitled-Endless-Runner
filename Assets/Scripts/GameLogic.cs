using System;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class GameLogic : MonoBehaviour
    {
        private byte _totalHearts = 4;
        public int totalHearts { get => _totalHearts; }

        public Action OnMainGameplayStarted;
        public Action<ObstacleStat> OnObstacleDetected;
        public Action OnPlayerHealthOver;

        private void OnEnable()
        {
            OnMainGameplayStarted += EnableObjects;
            OnPlayerHealthOver += EndGame;
        }

        private void OnDisable()
        {
            OnMainGameplayStarted -= EnableObjects;
            OnPlayerHealthOver -= EndGame;
        }

        private void EnableObjects()
        {

        }

        private void EndGame()
        {

        }
    }
}