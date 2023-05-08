#define MOBILE_CONTROLS                     //For Mobile Controls
//#define TEST_MODE
//#define TEST_CANVAS

using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
#if !MOBILE_CONTROLS
using UnityEngine.InputSystem;
#endif

namespace Untitled_Endless_Runner
{
    public enum PlayerAction { Jump, Slide, Dash, SpeedBoost, Hit }

    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D playerRB;
        [Range(8, 15f)]
        [SerializeField] private float jumpForce;
        [Range(3, 8f)]
        [SerializeField] private float slideForce, moveToOgPosForce;
        [SerializeField] private float heart = 4f, damageFromObstacle = 0.5f, tempPosX;               //By default, total hearts will be 4
        //[SerializeField] private Animator playerAnimator;
        [SerializeField] private SpriteRenderer playerRenderer;
        [SerializeField] private Animator playerAnimator;

        [Header("Local Refernce Script")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Player PowerUps")]
        [SerializeField] private int totalCoins;                    //Serialize For Test
        [SerializeField] private ObstacleStat heartStat;
        [SerializeField] private SpriteRenderer playerSprite;
        private Coroutine hurtCoroutine;

        [Space]
        private byte jumpCount;
        private bool hasJumped, isSliding, isDashing, disableSlide, disableJump, captured, gameStarted, atOgPos;        //, unAlive
        private Vector2 unAlivePos;
        [SerializeField] private GameObject mainCamera;

        [Header("Sound Effects")]
        [SerializeField] private AudioSource soundEffectsSource;
        [SerializeField] private AudioClip[] soundEffectsClips;

#if MOBILE_CONTROLS
        [Header("Mobile Controls")]
        private bool mobile_Jump, mobile_Slide;
#endif

        #region TestVariables
#if TEST_CANVAS
        [Header("Test Canvas")]
        [SerializeField] private TMP_Text debugTxt;
#endif
#endregion

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
            ReturnBackToOgPos();

#if TEST_CANVAS
            debugTxt = GameObject.Find("DebugText_TC3 (TMP)").GetComponent<TMP_Text>();
#endif
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
                //localGameLogic.OnPlayerSlide?.Invoke();
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
#elif TEST_CANVAS
        private void Update()
        {
            debugTxt.text = $"Player Controls\n" +
                $"disableSlide : {disableSlide}\n" +
                $"isDashing : {isDashing}\n" +
                $"disableJump : {disableJump}\n" +
                $"isSliding : {isSliding}\n" +
                $"heart : {heart}\n" +
                $"jumpCount : {jumpCount}\n";
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
                //Debug.Log($"Jumping. Jump Count : {jumpCount}");

                disableSlide = true;
                localGameLogic.OnPlayerAction?.Invoke(PlayerAction.Jump , 0);
                playerRB.velocity = transform.up * jumpForce;
                playerAnimator.Play("Jump", 0, 0f);
                jumpCount++;
                //playerAnimator.SetBool("Rotate", true);
                //Invoke(nameof(PlayAnimation), 1.01f);

                if (jumpCount == 3)
                {
                    hasJumped = true;
                    //Debug.Log($"Jumping. Jump Count : {jumpCount}, hasJumped : {hasJumped}");
                }

                //Debug.Log($"Disable Sliding : {disableSlide}");
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

        //On the Test Canvas
        public void ApplyForce()
        {
            playerRB.velocity = transform.up * jumpForce;
        }

        //On the ReturnToOGPos button, under the Test Canvas                    //On the UseDiamonds button, under the Buy Hearts Panel
        public void Alive(int heartsAmount)
        {
            localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Shield, 1);               //Give a shield to the player
            for (int i = 0; i < heartsAmount; i++)
            {
                localGameLogic.OnObstacleDetected?.Invoke(heartStat);                    //Give a heart to the player
            }
            playerRB.bodyType = RigidbodyType2D.Kinematic;
            playerAnimator.Play("GetUp" ,0 ,0f);
            _ = StartCoroutine(ReturnToOgPosUsnDmnds());
        }

        private void ReturnBackToOgPos()
        {
            if (!atOgPos)
            {
                if (transform.localPosition.x >= -5f)                
                    atOgPos = true;
                else
                    playerRB.AddForce(transform.right * moveToOgPosForce, ForceMode2D.Impulse);
                //playerRB.velocity = transform.right * jumpForce;
            }
            else if (transform.localPosition.x <= -5f)
                atOgPos = false;

            //Debug.Log($"CAlling Return Back, position : {transform.localPosition.x}");
            Invoke(nameof(ReturnBackToOgPos), 1f);              //Check back every 1 sec
        }

        private IEnumerator ReturnToOgPosUsnDmnds()
        {
            Vector2 startPos = transform.position;
            //Vector2 endPos = new Vector2(unAlivePos.x, -1f);
            Vector2 endPos = new Vector2(mainCamera.transform.position.x - 5.58f, mainCamera.transform.position.y - 3.7f);

            float time = 0f;
            while (true)
            {
                time += Time.deltaTime;

                if (time >= 1f)
                    break;

                transform.position = Vector2.Lerp(startPos, endPos, time);
                yield return null;
            }
            playerRB.bodyType = RigidbodyType2D.Dynamic;

            yield return new WaitForSeconds(0.4f);                  //This can cause problem because this toggle enableSpawn

            //Can combine below into a function 
            captured = false;
            gameStarted = true;
            GameManager.instance.gameStarted = true;
            jumpForce = 12f;                                            //Jump
            disableSlide = false;
            isDashing = false;
            isSliding = false;
            disableJump = false;
            jumpCount = 0;
        }

        private void ToggleAction(PlayerAction actionTaken, byte status)
        {
            if (status == 1)
            {
                switch (actionTaken)
                {
                    //Do Nothing
                    case PlayerAction.Jump:
                    case PlayerAction.SpeedBoost:
                    case PlayerAction.Hit:
                        break;

                    case PlayerAction.Slide:
                        {
                            isSliding = false;
                            disableJump = false;

                            break;
                        }

                    case PlayerAction.Dash:
                        {
                            disableSlide = false;
                            isDashing = false;

                            break;
                        }

                    default:
                        {
                            Debug.LogError($"Wait Index not found : {actionTaken.ToString()}");
                            break;
                        }
                }
            }
        }

        //This never gets executed
        private void ResetPlayerStats(int dummyData)
        {
            totalCoins = 0;
            gameStarted = false;
            //Debug.Log($"Un Alive set to false : {unAlive}");
            this.enabled = false;
            transform.GetChild(1).gameObject.SetActive(false);          //Shield
            jumpForce = 12f;                                            //Jump

            disableSlide = false;
            isDashing = false;
            isSliding = false;
            disableJump = false;
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
                //Reset Player Stats
                case 0:
                    {
                        //playerAnimator.Play("Idle", 0, 0f);
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
            if (jumpCount > 0 &&(collision.CompareTag("Steppable") || collision.CompareTag("Ground")))
            {
                if (jumpCount > 1)
                {
                    //Debug.Log($"Resetting under Trigger, jumpCount : {jumpCount}, Collision : {collision.tag}");
                    jumpCount = 0;              //To ensure that the player can double jump from the obstacle
                    disableSlide = false;

                    if (hasJumped)
                        hasJumped = false;
                    //Debug.Log($"Resetted under Trigger, jumpCount : {jumpCount}, Collision : {collision.tag}");
                }
                else                            //Increase counter when the player leaves the ground for the first time
                {
                    //Debug.Log($"Increasing jumpCount : {jumpCount}, Collision : {collision.tag}");
                    jumpCount++;                
                    //Debug.Log($"Increased jumpCount : {jumpCount}, Collision : {collision.tag}");
                }
            }

            if (gameStarted && collision.CompareTag("Captured"))
            {
                heart = 0f;
                captured = true;
                UnAlive();
                localGameLogic.OnPlayerCaptured?.Invoke();
                //Debug.Log($"Player captured : {captured}, gameStarted : {gameStarted}");
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
                        //Debug.Log($"Attack Obstacle : {obstacleStat.tag.ToString()}");

                        break;
                    }

                case ObstacleType.Boost:
                    {
                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.Fan:
                            case ObstacleTag.Trampoline:
                                {
                                    //Debug.Log($"Boost, Obstacle Tag : {obstacleStat.tag.ToString()}, jumpCount : {jumpCount}");

                                    if (jumpCount != 2)
                                    {
                                        jumpCount++;
                                        playerAnimator.Play("Jump", 0, 0f);
                                        disableSlide = true;
                                        //Debug.Log($"Jumping, disable Slide : {disableSlide}, jump Count : {jumpCount}");
                                    }

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
                            //Do Nothing
                            case ObstacleTag.Coin:
                            case ObstacleTag.Shield:
                            case ObstacleTag.Score2x:
                                break;

                            case ObstacleTag.Heart:
                                {
                                    heart++;
                                    break;
                                }

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
                case ObstacleTag.Score2x:
                case ObstacleTag.Dash:
                case ObstacleTag.SpeedBoost:
                case ObstacleTag.Heart:
                    break;

                case ObstacleTag.Coin:
                    {
                        if (amount == 1)
                        {
                            totalCoins++;
                            localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Coin, totalCoins);
                        }

                        break;
                    }

                case ObstacleTag.Shield:
                    {
                        if (amount == 1)
                            transform.GetChild(1).gameObject.SetActive(true);
                        else
                            transform.GetChild(1).gameObject.SetActive(false);
                        break;
                    }

                case ObstacleTag.HigherJump:
                    {
                        if (amount == 1)
                            jumpForce = 15f;
                        else
                            jumpForce = 12f;

                        break;
                    }

                default:
                    {
                        Debug.LogError($"Wrong Tag Detected for tag : {detectedTag}");
                        break;
                    }
            }
        }

        private void UnAlive()
        {
            //Debug.Log($"Player is unalived : {unAlive}");
            heart = 0f;                                     //In case, the player encounters SpikedHead or others of this type
            playerAnimator.Play("Hit", 0);
            unAlivePos = transform.position;
            playerRB.AddForce(transform.right * 12f, ForceMode2D.Impulse);

            int coinsBalance = PlayerPrefs.GetInt("COIN_AMOUNT", 0);
            PlayerPrefs.SetInt("COIN_AMOUNT", totalCoins + coinsBalance);
            GameManager.instance.coinsBalance = totalCoins + coinsBalance;
            Debug.Log("Coins Amount : " + PlayerPrefs.GetInt("COIN_AMOUNT", -1));
            localGameLogic.OnPlayerHealthOver?.Invoke(0);
            GameManager.instance.gameStarted = false;
            gameStarted = false;
            hasJumped = false;
        }

        public void TakeDamage()
        {
            soundEffectsSource.clip = soundEffectsClips[0];
            soundEffectsSource.Play();

            heart -= damageFromObstacle;

            if (heart <= 0f)
                UnAlive();

            if (hurtCoroutine != null)
                StopCoroutine(hurtCoroutine);
            playerSprite.color = Color.red;
            hurtCoroutine = StartCoroutine(ReturnSpriteToNormal());
            //Debug.Log($"Taking Damage : {heart}, Damage : {damageFromObstacle}, Aplha value : {playerSprite.color}");
        }

        private IEnumerator ReturnSpriteToNormal()
        {
            yield return new WaitForSeconds(0.1f);
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
            //Debug.Log($"Flashing, Player : {playerSprite.color}");
        }
    }
}
