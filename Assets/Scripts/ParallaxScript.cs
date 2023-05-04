using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class ParallaxScript : MonoBehaviour
    {
        private float length, startPos, tempPosXVal, distance;
        public GameObject cam;
        public float parallaxEffect;
        private int sceneMultiplier = 1, prevSceneIndex = 2;

        [Header("Local Refernce Script")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic.OnRestartFinished += CallReset;
        }

        private void OnDisable()
        {
            localGameLogic.OnRestartFinished -= CallReset;
        }

        // Start is called before the first frame update
        private void Start()
        {
            startPos = transform.position.x;
            length = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;
            prevSceneIndex = (transform.childCount - 1);                                    //The last scnen in line
            //Debug.Log($"Name : {transform.name}, Length : {length}, startpos : {startPos}");
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (GameManager.instance.gameStarted)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            tempPosXVal = (cam.transform.position.x * (1 - parallaxEffect));
            distance = (cam.transform.position.x * parallaxEffect);

            transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

            //Debug.Log($"tempPosXVal : {tempPosXVal}, transform.position.x : {transform.position.x}, name : {transform.name}");
            if (tempPosXVal > startPos + (length * sceneMultiplier))
            {
                float tempPosX = transform.GetChild(prevSceneIndex).transform.position.x;

                if (prevSceneIndex == (transform.childCount - 1))               // If the Scene at, the extreme end of the camera or last in line ,is about to go inside, then change to the one at the first in line
                    prevSceneIndex = 0;
                else
                    prevSceneIndex++;

                transform.GetChild(prevSceneIndex).transform.position =
                    new Vector3(tempPosX + length, transform.GetChild(prevSceneIndex).transform.position.y,
                    transform.GetChild(prevSceneIndex).transform.position.z);
                sceneMultiplier++;
                //length *= sceneMultiplier;
            }
            else if (tempPosXVal < startPos - length)
                startPos -= length;
        }

        private void ResetStats()
        {
            //Debug.Log($"Reset Stats Called from {transform.name}");
            sceneMultiplier = 1;
            prevSceneIndex = (transform.childCount - 1);                                    //The last scnen in line
            UpdatePosition();
        }

        private void CallReset()
        {
            Invoke(nameof(ResetStats), 1.2f);
        }
    }
}