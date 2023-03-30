using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Untitled_Endless_Runner
{
    public class SpikedHeadController : BaseObstacleController
    {
        [Header("Vertical Speeds")]
        [SerializeField] private float upSpeed;
        [SerializeField] private float downSpeed;

        [SerializeField] private float speedMultiplier = 0.02f;
        private float bottomPos, topPos, tempPos, time;
        private bool smashed, goingUp = true, enableVerticalMove = true;

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

            #region SpikeHeadVerticalMovement
            if (enableVerticalMove)
            {

                time += speedMultiplier * Time.deltaTime;

                if (time >= 1)
                {
                    tempPos = topPos;
                    topPos = bottomPos;
                    bottomPos = tempPos;
                    time = 0;

                    speedMultiplier = !goingUp ? upSpeed : downSpeed;
                    if (goingUp)
                    {
                        enableVerticalMove = false;
                        Invoke(nameof(EnableMove), 0.5f);
                    }
                    goingUp = !goingUp;
                }

                transform.position = new Vector2(transform.position.x, Mathf.Lerp(topPos, bottomPos, time));
            }
            #endregion SpikeHeadVerticalMovement
        }

        private void EnableMove()
        {
            enableVerticalMove = true;
        }

        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            if (!smashed)
            {
                smashed = true;
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
                Invoke(nameof(EnableEffectAgain), 1f);
                //player.GetComponent<PlayerController>().UnAlive();
            }
        }
    }
}