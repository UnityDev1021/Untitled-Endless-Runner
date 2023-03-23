using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SawController : BaseObstacleController
    {
        [SerializeField] private float damage;

        protected override void ApplyEffect(GameObject player)
        {
            //Debug.Log($"Applying Saw Effect");
            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}