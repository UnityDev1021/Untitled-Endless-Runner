using System;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    [CreateAssetMenu(fileName = "ObstacleGroup", menuName = "ObstacleGroup")]
    public class ObstacleGroup : ScriptableObject
    {
        public byte firstObstacleIndex;
        public byte secondObstacleindex;
        public float distance;
    }
}