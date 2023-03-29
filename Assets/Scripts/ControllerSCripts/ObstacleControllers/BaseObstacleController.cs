using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class BaseObstacleController : MonoBehaviour
    {
        [Header("Test Variables")]
        [SerializeField] private bool enableMove;

        [Header("Local References")]
        [SerializeField] protected GameLogic localGameLogic;

        [Space]
        //[SerializeField] private float moveSpeedMultiplier = -1f;
        public Transform cameraTrasform;
        [SerializeField] protected byte effectStatus;

        [SerializeField] private ObstacleStat _obstacleStat;
        public ObstacleStat obstacleStat { get { return _obstacleStat; } }

        protected virtual void Start()
        {
            cameraTrasform = GameManager.instance.cameraTransform;
            localGameLogic = GameManager.instance.gameLogicReference;
        }

        // Update is called once per frame
        protected virtual void FixedUpdate()
        {
            if (transform.position.x < (cameraTrasform.position.x - 12f))
                gameObject.SetActive(false);
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                //collision.transform.GetComponent<PlayerController>().ObstacleDetected(obstacleStat);

                if ((effectStatus == 0 && effectStatus != 2) || effectStatus >= 3)      //3 - Continuos, 2 - Nothing, 0 - Once
                {
                    ApplyEffect(collision.gameObject);
                    //Invoke("EnableEffectAgain", 1f);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                ClearEffects();
            }
        }

        //Restart the effect for reusing the obstacle
        protected void EnableEffectAgain()
        {
            effectStatus = 0;
        }

        protected virtual void ApplyEffect(GameObject player) { }
        protected virtual void ClearEffects() { }
        public virtual void AssignGroupTypes() { }
    }
}