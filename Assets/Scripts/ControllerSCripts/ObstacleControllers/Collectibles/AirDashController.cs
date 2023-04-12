using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class AirDashController : BaseObstacleController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            obstacleStat.activated = false;
        }

        protected override void ApplyEffect(GameObject player)
        {
            Debug.Log($"AirDash Collected");
            effectStatus = 1;
            obstacleStat.activated = true;
            //localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);                  //Not Doing Anything
            localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Dash, 1);
            Invoke(nameof(EnableEffectAgain), 0.5f);
            gameObject.SetActive(false);
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
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
    }
}