using System.Drawing;
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
                obstacleStat.activated = true;
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
                Invoke(nameof(ClearEffects), 1.5f);
            }
            //multiplier += 0.01f;
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

        protected override void ClearEffects()
        {
            effectStatus = 3;
            //multiplier = 0f;
        }
    }
}