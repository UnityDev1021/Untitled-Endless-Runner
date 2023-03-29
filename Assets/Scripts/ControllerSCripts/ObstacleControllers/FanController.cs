using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class FanController : BaseObstacleController
    {
        //private void Start()
        //{
        //    base.Start();
        //}

        [SerializeField] private float fanForce, multiplier;
        private float distanceFromPlayer;

        protected override void ApplyEffect(GameObject player)
        {
            //Debug.Log($"Applying Fan effect");
            distanceFromPlayer = Mathf.Abs(player.transform.position.y - transform.position.y);
            Vector2 force = new Vector2(0f, fanForce / distanceFromPlayer);
            //Vector2 force = new Vector2(0f, fanForce / distanceFromPlayer * distanceFromPlayer);
            player.GetComponent<Rigidbody2D>().AddForce(force * multiplier, ForceMode2D.Force);

            //As it is Fan, needs to be executed only once
            if (effectStatus == 3)
            {
                effectStatus = 4;
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat, 0f);
            }
            //multiplier += 0.01f;
        }

        protected override void ClearEffects()
        {
            //multiplier = 0f;
        }
    }
}