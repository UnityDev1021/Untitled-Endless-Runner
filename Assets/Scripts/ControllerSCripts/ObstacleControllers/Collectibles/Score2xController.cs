using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class Score2xController : BaseObstacleController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            obstacleStat.activated = false;
            EnableEffectAgain();
        }

        protected override void ApplyEffect(GameObject player)
        {
            //Debug.Log($"Score2x Collected");
            effectStatus = 1;
            obstacleStat.activated = true;
            //localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);                  //Not Doing Anything
            localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Score2x, 1);
            //Invoke(nameof(EnableEffectAgain), 0.5f);
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