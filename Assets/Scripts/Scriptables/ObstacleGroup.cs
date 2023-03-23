using System;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    [CreateAssetMenu(fileName = "ObstacleGroup", menuName = "ObstacleGroup")]
    public class ObstacleGroup : ScriptableObject
    {
        public byte[] obstaclesIndex;
        public float[] distance;
    }
}