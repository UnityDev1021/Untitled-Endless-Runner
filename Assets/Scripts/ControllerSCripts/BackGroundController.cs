using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [Range(0.05f, 0.5f)]
    [SerializeField] private float moveSpeed = 0.05f;

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Translate(new Vector3(moveSpeed , 0f, 0f));
    }
}
