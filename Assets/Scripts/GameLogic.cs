using System;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class GameLogic : MonoBehaviour
    {
        public Action OnMainGameplayStarted;
        public Action<ObstacleStat, float> OnObstacleDetected;
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