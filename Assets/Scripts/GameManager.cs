using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class GameManager : MonoBehaviour
    {
        [Header("Local Reference Scripts")]
        public GameLogic gameLogicReference;
        public PlayerController playerControllerReference;

        [Header("Local Reference Objects")]
        public Transform cameraTransform;

        [Header("Power Ups Section")]
        public int coinsBalance;
        public bool gameStarted, invincibility, speedBoost;
        public ObstacleStat[] tagsToBeDetected;

        [Header("Power Ups Section")]
        public int totalDiamonds;

        //[Header("PowerUps Section")]
        //protected bool ;

        private static GameManager _instance;
        public static GameManager instance
        {
            get => _instance;
        }

        // Start is called before the first frame update
        void Awake()
        {
            if (_instance == null && _instance != this) 
            {
                _instance = this;
            }
            else
                Destroy(this);


            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            totalDiamonds = PlayerPrefs.GetInt("DIAMONDS_AMOUNT", 0);
            coinsBalance = PlayerPrefs.GetInt("COIN_AMOUNT", 0);
        }
    }
}
