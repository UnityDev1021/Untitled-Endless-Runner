using System.Drawing;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class CoinController : BaseObstacleController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            obstacleStat.activated = false;
            EnableEffectAgain();
        }

        protected override void ApplyEffect(GameObject player)
        {
            //Debug.Log($"Coin Collected");
            effectStatus = 1;
            obstacleStat.activated = true;
            //localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
            localGameLogic.OnPowerUpCollected?.Invoke(ObstacleTag.Coin, 1);
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