using UnityEngine;
using UnityEngine.InputSystem;

namespace Untitled_Endless_Runner
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D playerRB;
        [Range(3, 8f)]
        [SerializeField] private float jumpForce;
        [SerializeField] private float health;
        [SerializeField] private Animator playerAnimator;

        private byte jumpCount;
        private bool hasJumped, unAlive;
        private int animationIndex;

        // Start is called before the first frame update
        void Start()
        {
            playerRB = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (Keyboard.current[Key.Space].wasPressedThisFrame && !hasJumped)
            {
                Debug.Log($"Jumping");
                playerRB.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                jumpCount++;
                playerAnimator.Play("Rotate", 0);
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

        private void PlayAnimation()
        {
            switch(animationIndex)
            {
                case 0:
                    {
                        playerAnimator.SetBool("Rotate", false);

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
            if (collision.CompareTag("Ground"))
            {
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
                        Debug.Log($"Attack Obstacle");

                        break;
                    }

                case ObstacleType.Boost:
                    {
                        Debug.Log($"Boost Obstacle");

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
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
                UnAlive();
        }
    }
}
