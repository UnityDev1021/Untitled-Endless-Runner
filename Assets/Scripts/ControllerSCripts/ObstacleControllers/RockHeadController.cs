using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Untitled_Endless_Runner
{
    public class RockHeadController : BaseObstacleController
    {
        [Header("Vertical Speeds")]
        [SerializeField] private float upSpeed;
        [SerializeField] private float downSpeed;

        [Space]
        [SerializeField] private float speedMultiplier = 0.6f;
        private float time, bottomPos, topPos, tempPos;
        private bool smashed, goingUp = true, enableVerticalMove = true;

        protected override void Start()
        {
            base.Start();
            bottomPos = -3.54f;
            topPos = -1.4f;

            //This is called after Assigning func is called
            if (tempPos == 0)
                tempPos = bottomPos;
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
                    goingUp = !goingUp;

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
                }
                
                transform.position = new Vector2(transform.position.x, Mathf.Lerp(tempPos, topPos, time));
            }
            #endregion RockHeadVerticalMovement
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
            switch (groupType)
            {
                case 1:
                    {
                        //time = 0.5f;
                        tempPos = -3.005f;
                        speedMultiplier = 0.875f;

                        break;
                    }

                case 2:
                    {
                        //time = 1f;
                        tempPos = -2.47f;
                        speedMultiplier = 1.05f;

                        break;
                    }

                case 3:
                    {
                        //time = 1f;
                        tempPos = -1.935f;
                        speedMultiplier = 1.225f;

                        break;
                    }

                default:
                    {
                        Debug.LogError($"GroupType not assigned for {obstacleStat.tag.ToString()}");

                        break;
                    }
            }

            enableVerticalMove = true;
            goingUp = true;
            time = 0f;

            transform.position = new Vector2(transform.position.x, Mathf.Lerp(tempPos, topPos, time));
            //Debug.Log($"Assigning groupType : {groupType}, tempPosY : {tempPosY}");
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
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
                Invoke(nameof(EnableEffectAgain), 1f);
            }
        }
    }
}