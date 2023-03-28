using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class ParallaxScript : MonoBehaviour
    {
        private float length, startPos, tempPosXVal, distance;
        public GameObject cam;
        public float parallaxEffect;
        private int sceneMultiplier = 1, prevSceneIndex = 2;

        // Start is called before the first frame update
        private void Start()
        {
            startPos = transform.position.x;
            length = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;
            prevSceneIndex = (transform.childCount - 1);                                    //The last scnen in line
            Debug.Log($"Name : {transform.name}, Length : {length}, startpos : {startPos}");
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            tempPosXVal = (cam.transform.position.x * (1 - parallaxEffect));
            distance = (cam.transform.position.x * parallaxEffect);

            transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

            //Debug.Log($"temp : {temp}, transform.position.x : {transform.position.x}");
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
    }
}