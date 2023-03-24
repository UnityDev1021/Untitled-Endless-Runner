using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class BaseObstacleController : MonoBehaviour
    {
        [Header("Test Variables")]
        [SerializeField] private bool enableMove;

        [Space]
        [SerializeField] private float moveSpeedMultiplier = -1f;
        public Transform cameraTrasform;
        protected bool appliedEffect;

        [SerializeField] private ObstacleStat _obstacleStat;
        public ObstacleStat obstacleStat { get { return _obstacleStat; } }

        protected virtual void Start()
        {
            cameraTrasform = GameManager.instance.cameraTransform;
        }

        // Update is called once per frame
        protected virtual void FixedUpdate()
        {
            if (enableMove)
            {
                if (transform.position.x > (cameraTrasform.position.x - 12f))
                {
                    transform.position = new Vector2(transform.position.x + (Time.deltaTime * moveSpeedMultiplier),
                                                    transform.position.y);
                    //transform.Translate(transform.right * moveSpeedMultiplier);
                }
                else
                    gameObject.SetActive(false);
            }
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player") && collision.transform.GetComponent<PlayerController>() != null)
            {
                collision.transform.GetComponent<PlayerController>().ObstacleDetected(obstacleStat);
                ApplyEffect(collision.gameObject);

                //if (!appliedEffect)
                //{
                //    //appliedEffect = true;
                //    //Invoke("EnableEffectAgain", 1f);
                //}
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player") && collision.transform.GetComponent<PlayerController>() != null)
            {
                ClearEffects();
            }
        }

        //Restart the effect for reusing the obstacle
        private void EnableEffectAgain()
        {
            appliedEffect = false;
        }

        protected virtual void ApplyEffect(GameObject player) { }
        protected virtual void ClearEffects() { }
        public virtual void AssignGroupTypes() { }
    }
}