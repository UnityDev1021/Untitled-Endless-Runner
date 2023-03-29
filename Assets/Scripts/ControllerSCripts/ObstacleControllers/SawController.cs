using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SawController : BaseObstacleController
    {
        [SerializeField] private float damage;

        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            //Debug.Log($"Applying Saw Effect");
            //player.GetComponent<PlayerController>().TakeDamage(damage);
            localGameLogic.OnObstacleDetected?.Invoke(obstacleStat, damage);
            Invoke(nameof(ClearEffects), 1f);
        }
    }
}