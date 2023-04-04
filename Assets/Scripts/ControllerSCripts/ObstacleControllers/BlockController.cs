using System.Collections;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class BlockController : BaseObstacleController
    {
        //[SerializeField] private byte mode = 0;
        [SerializeField] private float speedMultiplier = 0.7f, topPos, bottomPos;
        [SerializeField] private bool enableVerticalMove;
        private float time, tempPos;
        private Coroutine emergeOut;

        protected override void Start()
        {
            base.Start();

            emergeOut = StartCoroutine(EmergeOut());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (emergeOut != null)
                StopCoroutine(emergeOut);
        }

        public override void AssignGroupTypes(byte groupType, float dummyData)
        {
            switch (groupType)
            {
                case 0:
                case 1:
                    {
                        //Do Nothing
                        break;
                    }

                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    {
                        Destroy(this);

                        break;
                    }

                default:
                    {
                        Debug.LogError($"GroupType not assigned for {obstacleStat.tag.ToString()}");

                        break;
                    }
            }
            time = 0f;
        }

        private IEnumerator EmergeOut()
        {
            while (enableVerticalMove)
            {
                time += speedMultiplier * Time.deltaTime;

                if (time >= 1)
                {
                    tempPos = topPos;
                    topPos = bottomPos;
                    bottomPos = tempPos;
                    time = 0;
                }

                transform.parent.localPosition = new Vector2(0f, Mathf.Lerp(bottomPos, topPos, time));

                yield return null;
            }
        }
    }
}