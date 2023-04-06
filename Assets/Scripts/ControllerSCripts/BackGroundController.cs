using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Untitled_Endless_Runner;

public class BackGroundController : MonoBehaviour
{
    [Range(0.05f, 0.5f)]
    [SerializeField] private float moveSpeed = 0.05f;           //, manipulateSpeed;
    private bool scrollBackground = true;                      //Default is true
    private float time;

    [Header("Local Reference Script")]
    [SerializeField] private GameLogic localGameLogic;

    private void OnEnable()
    {
        localGameLogic.OnPlayerHealthOver += StopBackGroundScroll;
        localGameLogic.OnPlayerSlide += InvokeToggleSpeed;
    }

    private void OnDisable()
    {
        localGameLogic.OnPlayerHealthOver -= StopBackGroundScroll;
        localGameLogic.OnPlayerSlide -= InvokeToggleSpeed;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (scrollBackground)
            transform.Translate(new Vector3(moveSpeed , 0f, 0f));
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
            time += 1.1f * Time.deltaTime;

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
    }
}
