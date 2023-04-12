#define MOBILE_CONTROLS                     //For Mobile Controls
#define TEST_MODE

using UnityEngine;
using UnityEngine.InputSystem;

namespace Untitled_Endless_Runner
{
    public enum PlayerAction { Slide, Dash }

    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D playerRB;
        [Range(8, 15f)]
        [SerializeField] private float jumpForce;
        [Range(3, 8f)]
        [SerializeField] private float slideForce;
        [SerializeField] private float heart = 4f, damageFromObstacle = 0.5f, tempPosX;               //By default, total hearts will be 4
        //[SerializeField] private Animator playerAnimator;
        [SerializeField] private SpriteRenderer playerRenderer;
        [SerializeField] private Animator playerAnimator;

        [Header("Local Refernce Script")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Player PowerUps")]
        private int totalCoins;

        [Space]
        private byte jumpCount;
        private bool hasJumped, unAlive, isSliding, isDashing, firstJump, disableSlide, disableJump, disableDash;
        public bool gameStarted;

#if MOBILE_CONTROLS
        [Header("Mobile Controls")]
        private bool mobile_Jump, mobile_Slide;
#endif


        private void OnEnable()
        {
            localGameLogic.OnObstacleDetected += ObstacleDetected;
            localGameLogic.OnRestartClicked += PlayAnimation;
            localGameLogic.OnRestartClicked += ResetPlayerStats;
            localGameLogic.OnPowerUpCollected += TogglePowerUp;
            localGameLogic.OnPlayerAction += ToggleAction;
            //localGameLogic.OnRestartFinished += ResetPlayerStats;

#if TEST_MODE
            localGameLogic.FillUpPlayerHealth += RestorePlayerHearts;
#endif
        }

        private void OnDisable()
        {
            localGameLogic.OnObstacleDetected -= ObstacleDetected;
            localGameLogic.OnRestartClicked -= PlayAnimation;
            localGameLogic.OnRestartClicked -= ResetPlayerStats;
            localGameLogic.OnPowerUpCollected -= TogglePowerUp;
            localGameLogic.OnPlayerAction -= ToggleAction;
            //localGameLogic.OnRestartFinished -= ResetPlayerStats;

#if TEST_MODE
            localGameLogic.FillUpPlayerHealth -= RestorePlayerHearts;
#endif
        }

#if TEST_MODE
        private void RestorePlayerHearts()
        {
            heart = localGameLogic.totalHearts;
        }
#endif


        // Start is called before the first frame update
        void Start()
        {
            playerRB = GetComponent<Rigidbody2D>();
            heart = localGameLogic.totalHearts;
        }

#if !MOBILE_CONTROLS
        private void Update()
        {
            //Player can only jump or slide
            if (Keyboard.current[Key.Space].wasPressedThisFrame && !hasJumped && !disableJump)
            {
                Debug.Log($"Jumping");

                disableSlide = true;
                playerRB.velocity = transform.up * jumpForce;
                jumpCount++;
                playerAnimator.Play("Jump", 0, 0f);
                //playerAnimator.SetBool("Rotate", true);
                //Invoke(nameof(PlayAnimation), 1.01f);

                if (jumpCount == 1)
                    hasJumped = true;
            }
            else if (Keyboard.current[Key.S].wasPressedThisFrame && !isSliding && !disableSlide)             //Default was left Shift
            {
                Debug.Log($"Sliding");

                disableJump = true;
                isSliding = true;
                localGameLogic.OnPlayerSlide?.Invoke();
                playerAnimator.Play("Slide", 0, 0f);

                //tempPosX = transform.localPosition.x;               //Store current X Co-Ordinate
                //playerRB.velocity = transform.right * slideForce;                 //Manipulating BG

                Invoke(nameof(SetSlideOn), 1.25f);                  //Invoke after some time, as it takes time to slow down the moveSpeed of the BG_Controller
            }
            /*else if (isSliding)
            {
                if (playerRB.velocity.x <= 0.1f)
                {
                    if (transform.localPosition.x >= (tempPosX + 1.5f))
                    {
                        playerRB.AddForce(transform.right * slideForce * -3f, ForceMode2D.Force);           //Gravity Scale is 3f
                        //Debug.Log($"Applying Force");
                    }
                    else
                    {
                        isSliding = false;
                    }
                }
            }*/
        }
#endif

        public void SetStartPlayer()
        {
            gameStarted = true;
        }

        //On Jump button under the Main Gameplay Panel
        public void Mobile_Jump()
        {
            if (!hasJumped && !disableJump)
            {
                //Debug.Log($"Jumping");

                disableSlide = true;
                playerRB.velocity = transform.up * jumpForce;
                jumpCount++;
                playerAnimator.Play("Jump", 0, 0f);
                //playerAnimator.SetBool("Rotate", true);
                //Invoke(nameof(PlayAnimation), 1.01f);

                if (jumpCount == 1)
                    hasJumped = true;

                Debug.Log($"Disable Sliding : {disableSlide}");
            }
        }

        //On Slide button under the Main Gameplay Panel
        public void Mobile_Slide()
        {
            if (!isSliding && !disableSlide)
            {
                //Debug.Log($"Sliding");

                disableJump = true;
                isSliding = true;
                localGameLogic.OnPlayerAction?.Invoke(PlayerAction.Slide, 0);
                playerAnimator.Play("Slide", 0, 0f);

                //tempPosX = transform.localPosition.x;               //Store current X Co-Ordinate
                //playerRB.velocity = transform.right * slideForce;                 //Manipulating BG

                //This has to be set manually accordingly               //Not Needed Now
                //Invoke(nameof(SetSlideOn), 1.5f);                   //Invoke after some time, as it takes time to slow down the moveSpeed of the BG_Controller
            }
        }

        //On Dash button under the Main Gameplay Panel
        public void Mobile_Dash()
        {
            if (!isDashing && !isSliding)
            {
                //Debug.Log($"Sliding");
                disableSlide = true;
                isDashing = true;
                localGameLogic.OnPlayerAction?.Invoke(PlayerAction.Dash, 0);
                playerAnimator.Play("Dash", 0, 0f);
            }
        }

        private void ToggleAction(PlayerAction actionTaken, byte status)
        {
            switch (actionTaken)
            {
                case PlayerAction.Slide:
                    {
                        if (status == 1)
                        {
                            isSliding = false;
                            disableJump = false;
                        }
                        break;
                    }

                case PlayerAction.Dash:
                    {
                        if (status == 1)
                        {
                            disableSlide = false;
                            isDashing = false;
                        }
                        break;
                    }

                default:
                    {
                        Debug.LogError($"Wait Index not found : {actionTaken.ToString()}");
                        break;
                    }
            }
        }

        //This never gets executed
        private void ResetPlayerStats(int dummyData)
        {
            unAlive = false;
            gameStarted = false;
            Debug.Log($"Un Alive set to false : {unAlive}");
            this.enabled = false;
        }

        private void SetSlideOn()
        {
            isSliding = false;
            disableJump = false;
            //localGameLogic.OnPlayerSlide?.Invoke(false);
        }

        public void SetPlayerAfterEntry()
        {
            playerRenderer.maskInteraction = SpriteMaskInteraction.None;
        }

        private void PlayAnimation(int animationIndex)
        {
            switch(animationIndex)
            {
                case 0:
                    {
                        playerAnimator.Play("Idle", 0, 0f);
                        heart = localGameLogic.totalHearts;

                        break;
                    }

                default:
                    {
                        Debug.LogError($"Wrong Case {animationIndex} chosen for Player Controller under PlayAnimation");

                        break;
                    }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //This gets executed onc the player takes off from the ground as the player is still touching the ground.
            if (collision.CompareTag("Steppable") || collision.CompareTag("Ground"))
            {
                //Debug.Log($"Resetting under Trigger");
                jumpCount = 0;              //To ensure that the player can double jump from the obstacle
                disableSlide = false;

                if (hasJumped)
                    hasJumped = false;
            }

            if (gameStarted && collision.CompareTag("Captured"))
            {
                heart = 0f;
                UnAlive();
                localGameLogic.OnPlayerCaptured?.Invoke();
            }
        }

        public void ObstacleDetected(ObstacleStat obstacleStat)
        {
            //Debug.Log($"Obstacle Detected : {obstacleStat.tag}");
            switch (obstacleStat.type)
            {
                case ObstacleType.Attack:
                    {
                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.RockHead:
                            case ObstacleTag.SpikedHead:
                                {
                                    UnAlive();

                                    break;
                                }

                            case ObstacleTag.Saw:
                            case ObstacleTag.SpikedBall:
                            case ObstacleTag.Spike:
                            case ObstacleTag.Fire:
                                {
                                    TakeDamage();

                                    break;
                                }

                            default:
                                {
                                    Debug.LogError($"Attack Obstacle Not Found");

                                    break;
                                }
                        }
                        Debug.Log($"Attack Obstacle : {obstacleStat.tag.ToString()}");

                        break;
                    }

                case ObstacleType.Boost:
                    {
                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.Fan:
                            case ObstacleTag.Trampoline:
                                {
                                    if (jumpCount != 1)
                                        jumpCount++;

                                    break;
                                }

                            default:
                                {
                                    Debug.LogError($"Boost Obstacle Not Found");

                                    break;
                                }
                        }
                        //Debug.Log($"Boost Obstacle : {obstacleStat.tag}");

                        break;
                    }

                case ObstacleType.Normal:
                    {
                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.Block:
                                {

                                    break;
                                }

                            default:
                                {
                                    Debug.LogError($"Normal Obstacle Not Found");

                                    break;
                                }
                        }
                        //Debug.Log($"Boost Obstacle : {obstacleStat.tag}");

                        break;
                    }

                case ObstacleType.Normal_Attack:
                    {
                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.Block:
                                {

                                    break;
                                }

                            default:
                                {
                                    Debug.LogError($"Normal Obstacle Not Found");

                                    break;
                                }
                        }
                        //Debug.Log($"Boost Obstacle : {obstacleStat.tag}");

                        break;
                    }

                case ObstacleType.Power_Up:
                    {
                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.Coin:
                                {
                                    totalCoins++;
                                    localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Coin, totalCoins);

                                    break;
                                }

                            case ObstacleTag.Shield:
                                {
                                    transform.GetChild(1).gameObject.SetActive(true);

                                    break;
                                }

                            //Do Nothing
                            case ObstacleTag.Score2x:
                                break;

                            default:
                                {
                                    Debug.LogError($"Power-Up Not Found");

                                    break;
                                }
                        }

                        break;
                    }

                default:
                    {
                        Debug.LogError($"Unknown Type : {obstacleStat.type}");

                        break;
                    }
            }
        }

        private void TogglePowerUp(ObstacleTag detectedTag, int amount)
        {
            switch (detectedTag)
            {
                //Do Nothing
                case ObstacleTag.Coin:
                case ObstacleTag.Score2x:
                case ObstacleTag.Dash:
                    break;

                case ObstacleTag.Shield:
                    {
                        if (amount == 1)
                            transform.GetChild(1).gameObject.SetActive(true);
                        else
                            transform.GetChild(1).gameObject.SetActive(false);
                        break;
                    }

                default:
                    {
                        Debug.LogError($"Wrong Tag Detected for tag : {detectedTag}");
                        break;
                    }
            }
        }

        public void UnAlive()
        {
            unAlive = true;
            //Debug.Log($"Player is unalived : {unAlive}");
            localGameLogic.OnPlayerHealthOver?.Invoke();
            playerAnimator.Play("Hit", 0);
            playerRB.AddForce(transform.right * 12f, ForceMode2D.Impulse);


        }

        public void TakeDamage()
        {
            heart -= damageFromObstacle;

            if (heart <= 0f)
                UnAlive();

            Debug.Log($"Taking Damage : {heart}, Damage : {damageFromObstacle}");
        }
    }
}
