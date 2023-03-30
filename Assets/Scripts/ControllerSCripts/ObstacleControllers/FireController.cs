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
    }
}