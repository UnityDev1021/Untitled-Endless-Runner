using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SpikedBallController : BaseObstacleController
    {
        [SerializeField] private float damage, rotAngle, speedMultiplier = 0.02f;
        private float tempRot, leftRot, rightRot, currentAngle;
        public float time;

        protected override void FixedUpdate()
        {
            //Debug.Log($"Rotating Spikes");
            base.FixedUpdate();

            //transform.Translate(transform.right * 0.05f, Space.Self);
            //transform.position = new Vector2(transform.position.x + (transform.right.x * 0.05f), transform.position.y);

            #region SpikedBallMovement
            time += speedMultiplier * Time.deltaTime;

            if (time >= 1)
            {
                tempRot = leftRot;
                leftRot = rightRot;
                rightRot = tempRot;
                time = 0;
            }

            currentAngle = Mathf.Lerp(leftRot, rightRot, time);// * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            //transform.eulerAngles = new Vector3(0f, 0f, currentAngle);

            //currentAngle = rotAngle * Mathf.Sin(Time.time + (1f * speed));
            //transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            //transform.eulerAngles = new Vector2(transform.position.x,Mathf.Lerp(leftRotValue, rightRotValue, time));      //For swimming motion            
            #endregion SpikedBallMovement
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
            leftRot = rotAngle * -1f;
            rightRot = rotAngle;

            switch (groupType)
            {
                //Do Nothing
                case 0:
                    break;

                default:
                    {
                        Debug.LogError($"GroupType not assigned for {obstacleStat.tag.ToString()}");

                        break;
                    }
            }
        }

        protected override void ApplyEffect(GameObject player)
        {
            if (!invincibility)
            {
                effectStatus = 1;

                Debug.Log($"Applying Spike Effect");
                //player.GetComponent<PlayerController>().TakeDamage(damage);
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
                Invoke(nameof(EnableEffectAgain), 0.5f);
            }
        }
    }
}