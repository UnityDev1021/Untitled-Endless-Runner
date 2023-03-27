using System;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class GameLogic : MonoBehaviour
    {
        public Action OnMainGameplayStarted;

        private void OnEnable()
        {
            OnMainGameplayStarted += EnableObjects;
        }

        private void OnDisable()
        {
            OnMainGameplayStarted -= EnableObjects;            
        }

        private void EnableObjects()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}