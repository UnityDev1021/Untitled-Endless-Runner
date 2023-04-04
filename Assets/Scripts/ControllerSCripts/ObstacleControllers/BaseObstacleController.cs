using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class BaseObstacleController : MonoBehaviour
    {
        //[Header("Test Variables")]
        //[SerializeField] private bool enableMove;

        [Header("Local References")]
        [SerializeField] protected GameLogic localGameLogic;

        [Space]
        //[SerializeField] private float moveSpeedMultiplier = -1f;
        private bool enableEffects = true;                  //true by default
        public Transform cameraTransform;
        [SerializeField] protected byte effectStatus;

        [SerializeField] private ObstacleStat _obstacleStat;
        public ObstacleStat obstacleStat { get { return _obstacleStat; } }

        private void OnEnable()
        {
            Invoke(nameof(EnableActionFunctions), 0.1f);              //Apparently this works somehow.        //Might cause Race Condition
        }

        protected virtual void OnDisable()
        {
            Invoke(nameof(DisableActionFunctions), 0.1f);
        }

        protected virtual void Start() 
        {
            //Debug.Log($"Name : {transform.name},Dimension : {GetComponent<SpriteRenderer>().bounds.size}");                  //Will Cause error for some Prefabs such as SpikedBall
        }

        public void SetRefernces()
        {
            //Debug.Log($"Setting References");
            cameraTransform = GameManager.instance.cameraTransform;
            localGameLogic = GameManager.instance.gameLogicReference;
        }

        //helper function as the gamemanager isnt initialised as soon as the game starts
        private void EnableActionFunctions()
        {
            localGameLogic.OnPlayerHealthOver += SwitchEffectsOff;
            //Debug.Log($"Enabling");
        }

        //helper function as the gamemanager isnt initialised as soon as the game starts
        private void DisableActionFunctions()
        {
            localGameLogic.OnPlayerHealthOver -= SwitchEffectsOff;
            //Debug.Log($"Disabling");
        }

        // Update is called once per frame
        protected virtual void FixedUpdate()
        {
            if (transform.position.x < (cameraTransform.position.x - 12f))
                gameObject.SetActive(false);
        }

        private void SwitchEffectsOff()
        {
            enableEffects = false;
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (enableEffects && collision.transform.CompareTag("Player"))
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
        public virtual void AssignGroupTypes(byte groupType, float tempPosY) { }
    }
}