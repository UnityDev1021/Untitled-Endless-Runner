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
        private bool smashed, goingUp = true, enableMove = true, enableHorizontalMove = false;

        //Reset on Re-Use
        private void OnEnable()
        {
            smashed = false;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            #region RockHeadVerticalMovement
            if (enableMove)
            {
                time += speedMultiplier * Time.deltaTime;

                if (time >= 1)
                {

                    tempPos = topPos;
                    topPos = bottomPos;
                    bottomPos = tempPos;
                    time = 0;

                    if (!enableHorizontalMove)
                    {
                        goingUp = !goingUp;
                        speedMultiplier = goingUp ? upSpeed : downSpeed;
                        if (goingUp)
                        {
                            enableMove = false;
                            Invoke(nameof(EnableMove), 0.5f);
                        }
                    }
                }
                
                if (enableHorizontalMove)
                    transform.localPosition = new Vector2(Mathf.Lerp(bottomPos, topPos, time), transform.localPosition.y);
                else
                    transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(tempPos, topPos, time));
            }
            #endregion RockHeadVerticalMovement
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
            tempPos = bottomPos = -3.54f;
            topPos = -1.4f;

            switch (groupType)
            {
                //Do NOthing
                case 0:
                    break;

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

                case 4:
                    {
                        //time = 1f;
                        enableHorizontalMove = true;
                        bottomPos = -2.5f;
                        topPos = 2.5f;

                        break;
                    }

                default:
                    {
                        Debug.LogError($"GroupType not assigned for {obstacleStat.tag.ToString()}");

                        break;
                    }
            }

            enableMove = true;
            goingUp = true;
            time = 0f;

            if (enableHorizontalMove)
                transform.localPosition = new Vector2(Mathf.Lerp(bottomPos, topPos, time), transform.localPosition.y);
            else
                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(tempPos, topPos, time));
            //Debug.Log($"Assigning groupType : {groupType}, tempPosY : {tempPosY}");
        }

        private void EnableMove()
        {
            enableMove = true;
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