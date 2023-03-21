using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager instance
        {
            get => _instance;
        }

        // Start is called before the first frame update
        void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}
