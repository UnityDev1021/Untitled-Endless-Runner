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
            distanceFromPlayer = Mathf.Abs(player.transform.position.y - transform.position.y);
            Vector2 force = new Vector2(0f, fanForce / distanceFromPlayer);
            //Vector2 force = new Vector2(0f, fanForce / distanceFromPlayer * distanceFromPlayer);
            player.GetComponent<Rigidbody2D>().AddForce(force * multiplier, ForceMode2D.Force);

            //As it is Fan, needs to be executed only once
            if (effectStatus == 3)
            {
                //Debug.Log($"ACtivating, Effect Status : {effectStatus}");
                //effectStatus = 4;
                //obstacleStat.activated = true;
                localGameLogic.OnObstacleDetected?.Invoke(obstacleStat);
                //Invoke(nameof(ClearEffects), 1.5f);
                //Debug.Log($"ACtivated, Effect Status : {effectStatus}");
            }
            //multiplier += 0.01f;
            //Debug.Log($"Applying Fan effect, Force applied : {force}, Player velocity : {player.GetComponent<Rigidbody2D>().velocity}");
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
            //Debug.Log($"Clearing Effect, effect Status : {effectStatus}");
            effectStatus = 3;
            //Debug.Log($"Cleared Effect, effect Status : {effectStatus}");
            //multiplier = 0f;
        }
    }
}