using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SawController : BaseObstacleController
    {
        [SerializeField] private float damage, rotationSpeed = 6f;          //rotationSpeed will be 6 by default

        [Header ("Control Variables")]
        [SerializeField] private float speedMultiplier = 0.02f;
        private float bottomPos, topPos, tempPos, time;
        private bool enableVerticalMove = false;

        protected override void ApplyEffect(GameObject player)
        {
            if (!invincibility)
            {
                effectStatus = 1;

                //Debug.Log($"Applying Saw Effect");
                //player.GetComponent<PlayerController>().TakeDamage(damage);
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
                Invoke(nameof(EnableEffectAgain), 1f);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            transform.Rotate(new Vector3(0f, 0f, rotationSpeed));

            if (enableVerticalMove)
            {
                time += speedMultiplier * Time.deltaTime;

                if (time >= 1)
                {
                    tempPos = topPos;
                    topPos = bottomPos;
                    bottomPos = tempPos;
                    time = 0;
                }
                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(bottomPos, topPos, time));
            }
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
            switch (groupType)
            {
                //Do NOthing
                case 0:
                    break;

                case 1:
                    {
                        enableVerticalMove = true;

                        bottomPos = -4f;
                        topPos = -1f;

                        break;
                    }

                case 2:
                    {
                        enableVerticalMove = true;

                        topPos = -4f;
                        bottomPos = -1f;

                        break;
                    }

                default:
                    {
                        Debug.LogError($"GroupType not assigned for {obstacleStat.tag.ToString()}");

                        break;
                    }
            }
            time = 0f;

            transform.position = new Vector2(transform.position.x, Mathf.Lerp(bottomPos, topPos, time));
        }
    }
}