using System.Drawing;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class TrampolineController : BaseObstacleController
    {
        //trampolineForce is just to offset the gravity on the player
        [SerializeField] private float forceMultiplier = 3, trampolineForce = 5;                       //By default, forceMultiplier is 1, trampolineForce is 10
        [SerializeField] private Animator trampolineAnimator;

        protected override void ApplyEffect(GameObject player)
        {
            effectStatus = 1;

            //Debug.Log($"Applying Trampoline Effect");
            trampolineAnimator.Play("Activate", 0);
            player.GetComponent<Rigidbody2D>().AddForce(transform.up * trampolineForce * forceMultiplier, ForceMode2D.Impulse);
            //Debug.Log($"Player VElocity : {player.GetComponent<Rigidbody2D>().velocity}");
            obstacleStat.activated = true;
            localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
            Invoke(nameof(EnableEffectAgain), 0.5f);
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