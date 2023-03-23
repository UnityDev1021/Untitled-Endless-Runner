using Unity.VisualScripting;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SpikedBallController : BaseObstacleController
    {
        [SerializeField] private float damage, rotAngle, speedMultiplier = 0.02f;
        private float time, tempRot, leftRot, rightRot, currentAngle;

        protected override void Start()
        {
            base.Start();
            leftRot = rotAngle * -1f;
            rightRot = rotAngle;
        }

        protected override void FixedUpdate()
        {
            //Debug.Log($"Rotating Spikes");
            base.FixedUpdate();

            //transform.Translate(transform.right * 0.05f, Space.Self);
            //transform.position = new Vector2(transform.position.x + (transform.right.x * 0.05f), transform.position.y);

            #region SpikedBallMovement
            time += speedMultiplier * Time.deltaTime;

            if (time >= 100)
            {
                tempRot = leftRot;
                leftRot = rightRot;
                rightRot = tempRot;
                time = 0;
            }

            currentAngle = Mathf.Lerp(leftRot, rightRot, time) * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            //transform.eulerAngles = new Vector3(0f, 0f, currentAngle);

            //currentAngle = rotAngle * Mathf.Sin(Time.time + (1f * speed));
            //transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            //transform.eulerAngles = new Vector2(transform.position.x,Mathf.Lerp(leftRotValue, rightRotValue, time));      //For swimming motion            
            #endregion SpikedBallMovement
        }

        protected override void ApplyEffect(GameObject player)
        {
            Debug.Log($"Applying Spike Effect");
            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}