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
            //Debug.Log($"Applying Fan Effect");
            distanceFromPlayer = Mathf.Abs(player.transform.position.y - transform.position.y);
            Vector2 force = new Vector2(0f, fanForce / distanceFromPlayer);
            //Vector2 force = new Vector2(0f, fanForce / distanceFromPlayer * distanceFromPlayer);
            player.GetComponent<Rigidbody2D>().AddForce(force * multiplier, ForceMode2D.Force);
            //multiplier += 0.01f;
        }

        protected override void ClearEffects()
        {
            //multiplier = 0f;
        }
    }
}