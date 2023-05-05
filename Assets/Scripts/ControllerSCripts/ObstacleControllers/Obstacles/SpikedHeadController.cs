/***********************************************
 * The Top Pos will always be -1.4, bottom POs will always be -3.54, or according to the set data. Taking intervals divided by 4 from 100,
 * Intervals are :- 25% [-3.005y][0.875sp], 50% [-2.47y][1.05sp], 75% [-1.935y][1.225]
 ***********************************************/

using UnityEngine;
namespace Untitled_Endless_Runner
{
    public class SpikedHeadController : BaseObstacleController
    {
        [Header("Test Variables")]
        private bool testStartPos;

        [Header("Collider Controls")]
        public LayerMask playerLayerMask;
        public Collider2D powerUpCol, baseCol;

        [Header("Vertical Speeds")]
        [SerializeField] private float upSpeed;
        [SerializeField] private float downSpeed;

        [SerializeField] private float speedMultiplier = 0.02f;
        private float bottomPos, topPos, tempPos, time;
        private bool smashed, goingUp = true, enableMove = true, enableHorizontalMove = false, destroyed = false;       //, firstGo = true

        //Reset on Re-Use
        protected override void OnEnable()
        {
            base.OnEnable();
            smashed = false;
            EnableEffectAgain();
            destroyed = false;
            GetComponent<Animator>().Play("Nothing", 0, 0f);                //If the RockHead is smashed
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponent<BoxCollider2D>().enabled = true;
            //Debug.Log($"On Enable");
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            #region SpikeHeadVerticalMovement
            if (enableMove)
            {

                time += speedMultiplier * Time.deltaTime;
                //Debug.Log($"Before Global Pos : {transform.position}, Local Pos : {transform.localPosition}, tempPos : {tempPos}");

                if (time >= 1)
                {
                    //For the dirst time, the tempPos will be the custom one, after that the values will become default
                    tempPos = topPos;
                    topPos = bottomPos;
                    bottomPos = tempPos;
                    time = 0;

                    if (!enableHorizontalMove)
                    {
                        goingUp = !goingUp;

                        //By default, the SpikeHead will be moving up, so when it reaches up, then the speed should toggle.
                        //So, if goingUp is true by default, the speed should toggle to downSpeed when coming down/reached top position
                        speedMultiplier = goingUp ? upSpeed : downSpeed;
                        if (goingUp)               //Reached Ground
                        {
                            enableMove = false;
                            Invoke(nameof(EnableMove), 0.5f);
                        }
                    }
                    //Debug.Log($"Time's up, Going Up : {goingUp}");
                }

                if (enableHorizontalMove)
                    transform.localPosition = new Vector2(Mathf.Lerp(bottomPos, topPos, time), transform.localPosition.x);
                else
                    transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(tempPos, topPos, time));
                //Debug.Log($"After Global Pos : {transform.position}, Local Pos : {transform.localPosition}, tempPos : {tempPos}");
            }
            #endregion SpikeHeadVerticalMovement
        }

        private void EnableMove()
        {
            enableMove = true;
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
            tempPos = bottomPos = -3.54f;
            topPos = -1.4f;

            switch (groupType)
            {
                //Do Nothing
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

            //firstGo = true;
            enableMove = true;
            goingUp = true;
            time = 0f;
            //tempPos = tempPosY;               //Setting Above

            if (enableHorizontalMove)
                transform.localPosition = new Vector2(Mathf.Lerp(bottomPos, topPos, time), transform.localPosition.y);
            else
                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(tempPos, topPos, time));
            //Debug.Log($"Assigning groupType : {groupType}, tempPosY : {tempPosY}, tempPos : {tempPos}");
            //Debug.Log($"Assigning Speed Multiplier : {speedMultiplier}");
        }

        protected override void ApplyEffect(GameObject player)
        {
            //The Obstacle is destroyed
            if (GameManager.instance.invincibility && !destroyed && powerUpCol.IsTouchingLayers(playerLayerMask))            
            {
                //gameObject.SetActive(false);
                destroyed = true;
                GetComponent<Animator>().Play("Destroyed", 0, 0f);
                transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                //Debug.Log($"Detected Player : {player.name}");
                if (!smashed && powerUpCol.IsTouchingLayers(playerLayerMask))               //No LayerMask check as the player does not have anything to stand on, can add a small platform
                {
                    effectStatus = 1;
                    smashed = true;
                    localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
                    Invoke(nameof(EnableEffectAgain), 1f);
                    //player.GetComponent<PlayerController>().UnAlive();
                }
            }
        }

        protected override void ToggleEffects(int dummyData = 0)
        {
            base.ToggleEffects();
            smashed = false;
            GameManager.instance.invincibility = false;
        }
    }
}