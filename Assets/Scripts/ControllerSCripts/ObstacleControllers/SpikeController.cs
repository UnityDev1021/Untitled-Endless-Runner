using System.Drawing;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SpikeController : BaseObstacleController
    {
        [SerializeField] private float damage;
        //[SerializeField] private byte mode = 0;

        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            Debug.Log($"Applying Spike Effect");
            //player.GetComponent<PlayerController>().TakeDamage(damage);
            localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
            Invoke(nameof(EnableEffectAgain), 1f);
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