using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class TrampolineController : BaseObstacleController
    {
        [SerializeField] private float trampolineForceMultiplier;
        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            Debug.Log($"Applying Trampoline Effect");
            player.GetComponent<Rigidbody2D>().AddForce(transform.up * trampolineForceMultiplier, ForceMode2D.Impulse);
            Invoke(nameof(ClearEffects), 1f);
        }
    }
}