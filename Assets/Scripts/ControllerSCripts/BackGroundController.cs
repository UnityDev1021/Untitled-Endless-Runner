using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Untitled_Endless_Runner;

public class BackGroundController : MonoBehaviour
{
    [Range(0.05f, 0.5f)]
    [SerializeField] private float moveSpeed = 0.05f;           //, manipulateSpeed;
    private bool scrollBackground = true;                      //Default is true
    private float time;
    [SerializeField] private TMP_Text scoreTxt;
    private int score;

    [Header("BackGround Reference")]
    [SerializeField] private GameObject BackGround;
    [SerializeField] private float[] BackgroundPropsPos;

    [Header("Local Reference Script")]
    [SerializeField] private GameLogic localGameLogic;

    private void OnEnable()
    {
        localGameLogic.OnPlayerHealthOver += StopBackGroundScroll;
        localGameLogic.OnPlayerSlide += InvokeToggleSpeed;
        localGameLogic.OnRestartClicked += ResetEnvironmentProps;
    }

    private void OnDisable()
    {
        localGameLogic.OnPlayerHealthOver -= StopBackGroundScroll;
        localGameLogic.OnPlayerSlide -= InvokeToggleSpeed;
        localGameLogic.OnRestartClicked -= ResetEnvironmentProps;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (scrollBackground)
        {
            transform.Translate(new Vector3(moveSpeed , 0f, 0f));
            score = (int)MathF.Round(transform.position.x);
            scoreTxt.text = score.ToString();
        }
    }

    private void InvokeToggleSpeed()
    {
        moveSpeed = 0.2f;

        _ = StartCoroutine(ToggleSpeed(0.1f));                       //Default Speed        
    }

    private IEnumerator ToggleSpeed(float manipulateSpeed)
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            time += 1.2f * Time.deltaTime;

            if (time >= 1)
            {
                time = 0;
                break;
            }

            moveSpeed = Mathf.Lerp(moveSpeed, manipulateSpeed, time);
            yield return null;
        }
    }

    private void StopBackGroundScroll()
    {
        scrollBackground = false;
        localGameLogic.OnGameOver?.Invoke(score);
    }

    //On the TapToPlay button, under the MainMenuPanel
    public void StartBackGroundScroll()
    {
        scrollBackground = true;
    }

    private void ResetEnvironmentProps(int dummyData)
    {
        int totalGroups = BackGround.transform.childCount;
        Debug.Log($"Total Groups : {totalGroups}");

        for (int i = 6; i < totalGroups; i++)
        {
            int groupChildren = BackGround.transform.GetChild(i).childCount;
            float spaceMultiplier = BackGround.transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;

            Debug.Log($"groupChildren : {groupChildren}, spaceMultiplier : {spaceMultiplier}");

            for  (int j = 0; j < groupChildren; j++)
            {
                BackGround.transform.GetChild(i).GetChild(j).localPosition =
                    new Vector3(BackgroundPropsPos[i - 6] + (j * spaceMultiplier),
                    BackGround.transform.GetChild(i).GetChild(j).localPosition.y, 
                    BackGround.transform.GetChild(i).GetChild(j).localPosition.z);

                Debug.Log($"Props Name : {BackGround.transform.GetChild(i).GetChild(j).name}, " +
                    $"Local Position : {BackGround.transform.GetChild(i).GetChild(j).localPosition}");
            }
        }
    }

    //private Vector3 SetPosition()
    //{
    //    return new Vector3();
    //}
}
