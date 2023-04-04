using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class FireController : BaseObstacleController
    {
        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            //Debug.Log($"Applying Fire Effect");
            //player.GetComponent<PlayerController>().TakeDamage(damage);
            localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
            Invoke(nameof(EnableEffectAgain), 1f);
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
            float tempPosY = 0;

            switch (groupType)
            {
                case 1:
                    {
                        tempPosY = -2.3f;
                        transform.eulerAngles = new Vector3(0f, 0f, 180f);

                        break;
                    }

                case 2:
                    {
                        tempPosY = -2.3f;

                        break;
                    }

                case 3:
                    {
                        tempPosY = -2.3f;

                        break;
                    }

                default:
                    {
                        Debug.LogError($"GroupType not assigned for {obstacleStat.tag.ToString()}");

                        break;
                    }
            }

            transform.position = new Vector2(transform.position.x, tempPosY);
        }
    }
}