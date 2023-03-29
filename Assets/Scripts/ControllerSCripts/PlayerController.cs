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
        [SerializeField] private float health;
        //[SerializeField] private Animator playerAnimator;
        [SerializeField] private SpriteRenderer playerRenderer;

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

        public void ObstacleDetected(ObstacleStat obstacleStat, float damage)
        {
            switch (obstacleStat.type)
            {
                case ObstacleType.Attack:
                    {
                        switch (obstacleStat.tag)
                        {
                            case ObstacleTag.RockHead:
                                {
                                    UnAlive();

                                    break;
                                }

                            case ObstacleTag.Saw:
                                {
                                    TakeDamage(damage);

                                    break;
                                }

                            case ObstacleTag.SpikedBall:
                                {
                                    TakeDamage(damage);

                                    break;
                                }

                            case ObstacleTag.SpikedHead:
                                {
                                    UnAlive();

                                    break;
                                }

                            case ObstacleTag.Spike:
                                {
                                    TakeDamage(damage);

                                    break;
                                }

                            case ObstacleTag.Fire:
                                {

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
                                {
                                    //playerRB.AddForce(force * multiplier, ForceMode2D.Force);

                                    break;
                                }

                            case ObstacleTag.Trampoline:
                                {

                                    break;
                                }

                            default:
                                {
                                    Debug.LogError($"Boost Obstacle Not Found");

                                    break;
                                }
                        }
                        Debug.Log($"Boost Obstacle : {obstacleStat.tag}");

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
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
                UnAlive();

            Debug.Log($"Taking Damage : {health}, Damage : {damage}");
        }
    }
}
