using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SpikeController : BaseObstacleController
    {
        [SerializeField] private float damage;

        protected override void ApplyEffect(GameObject player)
        {
            Debug.Log($"Applying Spike Effect");
            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}