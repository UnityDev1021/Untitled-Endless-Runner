/***********************************************
 * The Top Pos will always be -1.4, bottom POs will always be -3.54, or according to the set data. Taking intervals divided by 4 from 100,
 * Intervals are :- 25% [-3.005y][0.875sp], 50% [-2.47y][1.05sp], 75% [-1.935y][1.225]
 ***********************************************/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Untitled_Endless_Runner
{
    public class SpikedHeadController : BaseObstacleController
    {
        [Header("Test Variables")]
        private bool testStartPos;

        [Header("Vertical Speeds")]
        [SerializeField] private float upSpeed;
        [SerializeField] private float downSpeed;

        [SerializeField] private float speedMultiplier = 0.02f;
        private float bottomPos, topPos, tempPos, time;
        private bool smashed, goingUp = true, enableVerticalMove = true;       //, firstGo = true

        protected override void Start()
        {
            base.Start();
            bottomPos = -3.54f;                     //Default Values
            topPos = -1.4f;                     //Default Values

            //This is called after Assigning func is called
            if (tempPos == 0)
                tempPos = bottomPos;

            //Debug.Log("Starting");
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
                //Debug.Log($"Before Global Pos : {transform.position}, Local Pos : {transform.localPosition}, tempPos : {tempPos}");

                if (time >= 1)
                {
                    goingUp = !goingUp;

                    //For the dirst time, the tempPos will be the custom one, after that the values will become default
                    tempPos = topPos;
                    topPos = bottomPos;
                    bottomPos = tempPos;
                    time = 0;

                    //By default, the SpikeHead will be moving up, so when it reaches up, then the speed should toggle.
                    //So, if goingUp is true by default, the speed should toggle to downSpeed when coming down/reached top position
                    speedMultiplier = goingUp ? upSpeed : downSpeed;
                    if (goingUp)               //Reached Ground
                    {
                        enableVerticalMove = false;
                        Invoke(nameof(EnableMove), 0.5f);
                    }

                    //Debug.Log($"Time's up, Going Up : {goingUp}");
                }

                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(tempPos, topPos, time));
                //Debug.Log($"After Global Pos : {transform.position}, Local Pos : {transform.localPosition}, tempPos : {tempPos}");
            }
            #endregion SpikeHeadVerticalMovement
        }

        private void EnableMove()
        {
            enableVerticalMove = true;
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

            //firstGo = true;
            enableVerticalMove = true;
            goingUp = true;
            time = 0f;
            //tempPos = tempPosY;               //Setting Above

            transform.position = new Vector2(transform.position.x, Mathf.Lerp(tempPos, topPos, time));
            //Debug.Log($"Assigning groupType : {groupType}, tempPosY : {tempPosY}, tempPos : {tempPos}");
            //Debug.Log($"Assigning Speed Multiplier : {speedMultiplier}");
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