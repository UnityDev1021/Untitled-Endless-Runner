using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SawController : BaseObstacleController
    {
        [SerializeField] private float damage, rotationSpeed = 6f;          //rotationSpeed will be 6 by default

        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            //Debug.Log($"Applying Saw Effect");
            //player.GetComponent<PlayerController>().TakeDamage(damage);
            localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
            Invoke(nameof(EnableEffectAgain), 1f);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            transform.Rotate(new Vector3(0f, 0f, rotationSpeed));
        }
    }
}