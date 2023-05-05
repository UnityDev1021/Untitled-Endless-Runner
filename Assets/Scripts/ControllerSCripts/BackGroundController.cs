//#define TEST_MODE
#define TEST_CANVAS

using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class BackGroundController : MonoBehaviour
    {
        [Range(0.05f, 0.5f)]
        [SerializeField] private float moveSpeed = 0.05f;           //, manipulateSpeed;
        private bool scrollBackground = true, enableScore2x = false;                      //Default is true
        private float time;
        [SerializeField] private TMP_Text scoreTxt;
        private int score;

        [Header("BackGround Reference")]
        [SerializeField] private float[] BackgroundPropsPos;
        [SerializeField] private GameObject BackGround, mainCamera;

        [Header("Local Reference Script")]
        [SerializeField] private GameLogic localGameLogic;

        #region TestVariables
        [Header("Test Canvas")]
#if TEST_CANVAS
        [SerializeField] private TMP_Text debugTxt;
#endif
        #endregion

        private void OnEnable()
        {
            localGameLogic.OnPlayerHealthOver += StopBackGroundScroll;
            localGameLogic.OnRestartClicked += CallResetEnvironment;
            localGameLogic.OnRestartFinished += ResetPowerUpStats;
            localGameLogic.OnPowerUpCollected += CheckPowerUp;
            localGameLogic.OnPlayerAction += InvokeToggleSpeed;
            localGameLogic.OnGameplayContinued += InvokeBGScrolling;
        }

        private void OnDisable()
        {
            localGameLogic.OnPlayerHealthOver -= StopBackGroundScroll;
            localGameLogic.OnRestartClicked -= CallResetEnvironment;
            localGameLogic.OnRestartFinished -= ResetPowerUpStats;
            localGameLogic.OnPowerUpCollected -= CheckPowerUp;
            localGameLogic.OnPlayerAction -= InvokeToggleSpeed;
            localGameLogic.OnGameplayContinued -= InvokeBGScrolling;
        }

#if TEST_CANVAS
        private void Start()
        {
            debugTxt = GameObject.Find("DebugText_TC4 (TMP)").GetComponent<TMP_Text>();
        }
#endif

#if TEST_CANVAS
        private void Update()
        {
            debugTxt.text = $"enableScore2x : {enableScore2x}\n" +
                $"moveSpeed : {moveSpeed}" +
                $"scrollBackground : {scrollBackground}";
        }
#endif

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (scrollBackground)
            {
#if TEST_MODE
#else
                transform.Translate(new Vector3(moveSpeed, 0f, 0f));
#endif
                score = (int)MathF.Round(transform.position.x);

                if (enableScore2x)                  //As for each forward x position, we get 1 point, so x+1 point for multiplier. Can make scoreMultiplier if needs be.            
                    score++;

                scoreTxt.text = score.ToString();
            }
        }

        private void CheckPowerUp(ObstacleTag detectedTag, int amount)
        {
            switch (detectedTag)
            {
                //Do Nothing
                case ObstacleTag.Coin:
                case ObstacleTag.Shield:
                case ObstacleTag.Dash:
                case ObstacleTag.HigherJump:
                case ObstacleTag.Heart:
                    break;

                case ObstacleTag.Score2x:
                    {
                        if (amount == 1)
                            enableScore2x = true;
                        else
                            enableScore2x = false;

                        break;
                    }

                case ObstacleTag.SpeedBoost:
                    {
                        if (amount == 1 && !GameManager.instance.speedBoost)
                        {
                            GameManager.instance.speedBoost = true;
                            moveSpeed *= 1.5f;
                            _ = StartCoroutine(ToggleSpeed(8f, PlayerAction.SpeedBoost, 1f));                      //Default Speed
                        }

                        break;
                    }
            }
        }

        private void InvokeToggleSpeed(PlayerAction actionTaken, byte status)
        {
            if (status == 0)
            {
                switch (actionTaken)
                {
                    //Do Nothing
                    case PlayerAction.Hit:
                    case PlayerAction.Jump:
                        break;

                    case PlayerAction.Slide:
                        {
                            moveSpeed = 0.2f;
                            _ = StartCoroutine(ToggleSpeed(0.5f, actionTaken, 0.8f));                       //Default Speed

                            break;
                        }

                    case PlayerAction.Dash:
                        {
                            moveSpeed = 0.4f;
                            _ = StartCoroutine(ToggleSpeed(0.2f, actionTaken, 0.8f));                       //Default Speed

                            break;
                        }

                    default:
                        {
                            Debug.LogError($"Power Up not found : {actionTaken}");
                            break;
                        }
                }
            }

            //Debug.Log($"Start Move Speed : {moveSpeed}");    
        }

        private IEnumerator ToggleSpeed(float waitTime, PlayerAction actionTaken, float totalTime)
        {
            yield return new WaitForSeconds(waitTime);

            while (true)
            {
                time += 1.2f * Time.deltaTime;

                if (time >= totalTime)
                {
                    time = 0;
                    GameManager.instance.speedBoost = false;
                    localGameLogic.OnPlayerAction?.Invoke(actionTaken, 1);

                    /*switch (actionTaken)
                    {
                        case PlayerAction.Slide:
                            {
                                break;
                            }

                        case PlayerAction.Dash:
                            {
                                yield return new WaitForSeconds(0.2f);
                                break;
                            }

                        default:
                            {
                                Debug.LogError($"Wait Index not found : {actionTaken.ToString()}");
                                break;
                            }
                    }*/

                    break;
                }

                moveSpeed = Mathf.Lerp(moveSpeed, 0.1f, time);
                //Debug.Log($"Move Speed : {moveSpeed}");

                yield return null;
            }
        }

        private void StopBackGroundScroll(int restartStatus)
        {
            scrollBackground = false;
             
            if (restartStatus == 0)
                localGameLogic.OnGameOver?.Invoke(score);
        }

        //On the TapToPlay button, under the MainMenuPanel
        public void StartBackGroundScroll()
        {
            scrollBackground = true;
        }

        private void InvokeBGScrolling()
        {
            Invoke(nameof(StartBackGroundScroll), 1.5f);
        }

        private void CallResetEnvironment(int dummyData)
        {
            Invoke(nameof(ResetEnvironmentProps), 1.1f);
        }

        private void ResetEnvironmentProps()
        {
            mainCamera.transform.position = new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z);

            Debug.Log($"Restart Clicked");
            int totalGroups = BackGround.transform.childCount;
            //Debug.Log($"Total Groups : {totalGroups}");

            for (int i = 6; i < totalGroups; i++)
            {
                int groupChildren = BackGround.transform.GetChild(i).childCount;
                float spaceMultiplier = BackGround.transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;

                //Debug.Log($"groupChildren : {groupChildren}, spaceMultiplier : {spaceMultiplier}");

                for (int j = 0; j < groupChildren; j++)
                {
                    BackGround.transform.GetChild(i).GetChild(j).localPosition =
                        new Vector3(BackgroundPropsPos[i - 6] + (j * spaceMultiplier),
                        BackGround.transform.GetChild(i).GetChild(j).localPosition.y,
                        BackGround.transform.GetChild(i).GetChild(j).localPosition.z);

                    //Debug.Log($"Props Name : {BackGround.transform.GetChild(i).GetChild(j).name}, " +
                    //    $"Local Position : {BackGround.transform.GetChild(i).GetChild(j).localPosition}");
                }
            }

            this.enabled = false;
        }

        private void ResetPowerUpStats()
        {
            moveSpeed = 0.1f;
            GameManager.instance.speedBoost = false;
            enableScore2x = false;
        }

        //private Vector3 SetPosition()
        //{
        //    return new Vector3();
        //}
    }
}