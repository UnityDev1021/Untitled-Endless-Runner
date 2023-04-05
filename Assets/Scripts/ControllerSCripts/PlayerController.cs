using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Untitled_Endless_Runner
{
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

        [Space]
        private byte jumpCount;
        private bool hasJumped, unAlive, isSliding;
        public bool gameStarted;
        private int animationIndex;

        private void OnEnable()
        {
            localGameLogic.OnObstacleDetected += ObstacleDetected;
        }

        private void OnDisable()
        {
            localGameLogic.OnObstacleDetected -= ObstacleDetected;            
        }

        // Start is called before the first frame update
        void Start()
        {
            playerRB = GetComponent<Rigidbody2D>();
            heart = localGameLogic.totalHearts;
        }

        private void Update()
        {
            //Player can only jump or slide
            if (Keyboard.current[Key.Space].wasPressedThisFrame && !hasJumped)
            {
                Debug.Log($"Jumping");
                playerRB.velocity = transform.up * jumpForce;
                jumpCount++;
                playerAnimator.Play("Jump", 0, 0f);
                //playerAnimator.SetBool("Rotate", true);
                animationIndex = 0;
                //Invoke(nameof(PlayAnimation), 1.01f);

                if (jumpCount == 2)
                {
                    jumpCount = 0;
                    hasJumped = true;
                }
            }
            else if (Keyboard.current[Key.LeftShift].wasPressedThisFrame && !isSliding)
            {
                isSliding = true;
                tempPosX = transform.localPosition.x;               //Store current X Co-Ordinate
                playerAnimator.Play("Slide", 0, 0f);
                playerRB.velocity = transform.right * slideForce;
            }
            else if (isSliding)
            {
                if (playerRB.velocity.x <= 0.1f)
                {
                    if (transform.localPosition.x >= (tempPosX + 1.5f))
                    {
                        playerRB.AddForce(transform.right * slideForce * -3f, ForceMode2D.Force);           //Gravity Scale is 3f
                        Debug.Log($"Applying Force");
                    }
                    else
                    {
                        isSliding = false;
                    }
                }
            }

        }

        public void SetPlayerAfterEntry()
        {
            playerRenderer.maskInteraction = SpriteMaskInteraction.None;
        }

        private void PlayAnimation()
        {
            switch(animationIndex)
            {
                case 0:
                    {
                        //playerAnimator.SetBool("Rotate", false);

                        break;
                    }

                default:
                    {
                        Debug.Log($"Wrong Case {animationIndex} chosen for Player Controller under PlayAnimation");

                        break;
                    }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ground") || collision.CompareTag("Steppable"))
            {
                jumpCount = 0;              //To ensure that the player can double jump from the obstacle

                if (hasJumped)
                {
                    hasJumped = false;
                }
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

                default:
                    {
                        Debug.LogError($"Unknown Type : {obstacleStat.type}");

                        break;
                    }
            }
        }

        public void UnAlive()
        {
            unAlive = true;
            Debug.Log($"Player is unalived : {unAlive}");
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
