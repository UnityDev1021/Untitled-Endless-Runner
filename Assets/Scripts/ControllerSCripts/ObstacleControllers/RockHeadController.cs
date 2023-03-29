using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Untitled_Endless_Runner
{
    public class RockHeadController : BaseObstacleController
    {
        [Header("Test Variables")]
        [SerializeField] private float upSpeed;
        [SerializeField] private float downSpeed;

        [Space]
        [SerializeField] private float speedMultiplier = 0.6f;
        private float time, bottomPos, topPos, tempPos;
        private bool smashed, goingUp = false, enableVerticalMove = true;

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

                transform.position = new Vector2(transform.position.x, Mathf.Lerp(bottomPos, topPos, time));
            }
            #endregion RockHeadVerticalMovement
        }

        private void EnableMove()
        {
            enableVerticalMove = true;
        }

        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            if (!smashed && goingUp)                //Apparently this is the opposite
            {
                smashed = true;
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat, 0f);
                Invoke(nameof(ClearEffects), 1f);
            }
        }
    }
}