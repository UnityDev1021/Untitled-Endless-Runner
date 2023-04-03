using System.Collections;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class MetalPlateController : BaseObstacleController
    {
        //[SerializeField] private byte mode = 0;
        [SerializeField] private float emergeTime = 0.5f, speedMultiplier = 0.7f, topPos, bottomPos;
        private byte fullyEmerged;
        private float time, tempPos;
        private Coroutine emergeOut;

        protected override void Start()
        {
            base.Start();

            _ = StartCoroutine(EmergeOut());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (emergeOut != null )
                StopCoroutine(emergeOut);
        }

        private IEnumerator EmergeOut()
        {
            fullyEmerged = 0;
            GameObject spikes = transform.GetChild(0).gameObject;

            while (fullyEmerged < 2)
            {
                time += speedMultiplier * Time.deltaTime;

                if (time >= 1)
                {
                    tempPos = topPos;
                    topPos = bottomPos;
                    bottomPos = tempPos;
                    time = 0;
                    fullyEmerged++;
                }

                spikes.transform.localPosition = new Vector2(0f, Mathf.Lerp(bottomPos, topPos, time));
                
                yield return null;
            }

            Invoke(nameof(CallEmergeFunc), emergeTime);
        }

        private void CallEmergeFunc()
        {
            emergeOut = StartCoroutine(EmergeOut());
        }
    }
}