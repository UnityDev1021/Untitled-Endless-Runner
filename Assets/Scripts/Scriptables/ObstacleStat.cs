using UnityEngine;

namespace Untitled_Endless_Runner
{
    public enum ObstacleType { Attack, Boost }
    public enum ObstacleTag { Block, RockHead, Saw, SpikedBall, SpikedHead, Spike, Fan, Trampoline, Fire }

    [CreateAssetMenu(fileName = "ObstacleStat", menuName = "ObstacleStat")]
    public class ObstacleStat : ScriptableObject
    {
        public ObstacleType type;
        public ObstacleTag tag;
        public bool activated;
    }
}