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
        [SerializeField] private float heart = 4f, damageFromObstacle = 0.5f;               //By default, total hearts will be 4
        //[SerializeField] private Animator playerAnimator;
        [SerializeField] private SpriteRenderer playerRenderer;
        [SerializeField] private Animator playerAnimator;

        [Header("Local Refernce Script")]
        [SerializeField] private GameLogic localGameLogic;

        [Space]
        private byte jumpCount;
        private bool hasJumped, unAlive;
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
            if (Keyboard.current[Key.Space].wasPressedThisFrame && !hasJumped)
            {
                //Debug.Log($"Jumping");
                playerRB.velocity = transform.up * jumpForce;
                jumpCount++;
                //playerAnimator.Play("Rotate", 0);
                //playerAnimator.SetBool("Rotate", true);
                animationIndex = 0;
                Invoke(nameof(PlayAnimation), 1.01f);

                if (jumpCount == 2)
                {
                    jumpCount = 0;
                    hasJumped = true;
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
                            case ObstacleTag.Block:
                                {

                                    break;
                                }

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
