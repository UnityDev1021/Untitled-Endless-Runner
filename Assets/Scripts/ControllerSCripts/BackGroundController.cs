using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Untitled_Endless_Runner;

public class BackGroundController : MonoBehaviour
{
    [Range(0.05f, 0.5f)]
    [SerializeField] private float moveSpeed = 0.05f;
    private bool scrollBackground = true;                      //Default is true

    [Header("Local Reference Script")]
    [SerializeField] private GameLogic localGameLogic;

    private void OnEnable()
    {
        localGameLogic.OnPlayerHealthOver += StopBackGroundScroll;
    }

    private void OnDisable()
    {
        localGameLogic.OnPlayerHealthOver -= StopBackGroundScroll;        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (scrollBackground)
            transform.Translate(new Vector3(moveSpeed , 0f, 0f));
    }

    private void StopBackGroundScroll()
    {
        scrollBackground = false;
    }
}
