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
        private bool smashed, goingUp = false, enableVerticalMove = true, firstGo = true;

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
            firstGo = true;
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

                    if (firstGo)
                        firstGo = false;

                    speedMultiplier = !goingUp ? upSpeed : downSpeed;
                    if (goingUp)
                    {
                        enableVerticalMove = false;
                        Invoke(nameof(EnableMove), 0.5f);
                    }
                    goingUp = !goingUp;
                }

                if (firstGo)
                    transform.position = new Vector2(transform.position.x, Mathf.Lerp(tempPos, topPos, time));
                else
                    transform.position = new Vector2(transform.position.x, Mathf.Lerp(bottomPos, topPos, time));
            }
            #endregion RockHeadVerticalMovement
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