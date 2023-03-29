using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Untitled_Endless_Runner
{
    public class SpikedHeadController : BaseObstacleController
    {
        [SerializeField] private float speedMultiplier = 0.02f;
        private float bottomPos, topPos, tempPos;
        private bool smashed;

        public float time;

        protected override void Start()
        {
            base.Start();
            bottomPos = -3.54f;
            topPos = -1.4f;
        }

        //Reset on Re-Use
        private void OnEnable()
        {
            smashed = false;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            #region RockHeadVerticalMovement
            time += speedMultiplier * Time.deltaTime;

            if (time >= 1) 
            {
                tempPos = topPos;
                topPos = bottomPos;
                bottomPos = tempPos;
                time = 0;
            }

            transform.position = new Vector2(transform.position.x, Mathf.Lerp(topPos, bottomPos, time));
            #endregion RockHeadVerticalMovement
        }

        //protected override void OnTriggerStay2D(Collider2D collision)
        //{

        //}

        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            if (!smashed)
            {
                smashed = true;
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat, 0f);
                Invoke(nameof(ClearEffects), 1f);
                //player.GetComponent<PlayerController>().UnAlive();
            }
        }
    }
}