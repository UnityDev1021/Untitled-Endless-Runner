using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class ObstacleController : MonoBehaviour
    {
        [SerializeField] private float moveSpeedMultiplier = -0.01f;
        public Transform cameraTrasform;

        [SerializeField] private ObstacleStat _obstacleStat;
        public ObstacleStat obstacleStat { get { return _obstacleStat; } }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (transform.position.x > (cameraTrasform.position.x - 12f))
                transform.Translate(transform.right * moveSpeedMultiplier);
            else
                gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player") && collision.transform.GetComponent<PlayerController>() != null)
            {
                collision.transform.GetComponent<PlayerController>().ObstacleDetected(obstacleStat);
            }
        }
    }
}