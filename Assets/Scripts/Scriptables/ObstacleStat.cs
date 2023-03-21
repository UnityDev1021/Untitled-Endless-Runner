using UnityEngine;

namespace Untitled_Endless_Runner
{
    public enum ObstacleType { Attack, Boost }

    [CreateAssetMenu(fileName = "ObstacleStat", menuName = "ObstacleStat")]
    public class ObstacleStat : ScriptableObject
    {
        public ObstacleType type;
        public string tag;
    }
}