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
        private bool smashed, goingUp = true, enableVerticalMove = true, firstGo = true;

        protected override void Start()
        {
            base.Start();
            tempPos = bottomPos = -3.54f;
            topPos = -1.4f;
        }

        //Reset on Re-Use
        private void OnEnable()
        {
            smashed = false;
            //Debug.Log($"On Enable");
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

                    speedMultiplier = goingUp ? upSpeed : downSpeed;
                    if (goingUp)
                    {
                        enableVerticalMove = false;
                        Invoke(nameof(EnableMove), 0.5f);
                    }
                    goingUp = !goingUp;

                    Debug.Log($"Time's up, Going Up : {goingUp}");
                }

                transform.position = new Vector2(transform.position.x, Mathf.Lerp(tempPos, topPos, time));
            }
            #endregion SpikeHeadVerticalMovement
        }

        private void EnableMove()
        {
            enableVerticalMove = true;
        }

        public override void AssignGroupTypes(byte groupType, float tempPosY)
        {
            switch (groupType)
            {
                case 1:
                    {
                        //time = 0.5f;
                        tempPos = tempPosY;

                        break;
                    }

                case 2:
                    {
                        //time = 1f;

                        break;
                    }

                default:
                    {
                        Debug.LogError($"GroupType not assigned for {obstacleStat.tag.ToString()}");

                        break;
                    }
            }

            firstGo = true;
            enableVerticalMove = true;
            goingUp = true;
            time = 0f;

            transform.position = new Vector2(transform.position.x, Mathf.Lerp(tempPos, topPos, time));
            Debug.Log($"Assigning groupType : {groupType}, tempPosY : {tempPosY}");
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