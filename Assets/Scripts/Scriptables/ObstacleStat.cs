using UnityEngine;

namespace Untitled_Endless_Runner
{
    /**************************************************************************************************************************
     * Obstacles with 2 or more properties will be specified with tags, that are named as, the top/first or more, obstacle being the first tag 
     * and the bottom/second or more, obstacle being second tag.
     * 
     * => Attack - For normal attack obstacles
     * => Boost - For normal boost obstacles
     * => Normal - For normal obstacles with no power such as block
     * => Normal_Attack - For obstacles with both normal and attack.
     **************************************************************************************************************************/
    public enum ObstacleType { Attack, Boost, Normal, Normal_Attack, Power_Up }
    [SerializeField] public enum ObstacleTag { Block, RockHead, Saw, SpikedBall, SpikedHead, Spike, Fan, Trampoline, Fire, Block_Spike, 
        MetalPlate_Spike, Coin, Shield, Score2x, Dash, HigherJump, SpeedBoost, Heart }

    public enum ObstacleCombo { Single, Multiple, Multiple_Child }

    [CreateAssetMenu(fileName = "ObstacleStat", menuName = "ObstacleStat")]
    public class ObstacleStat : ScriptableObject
    {
        public ObstacleType type;
        public ObstacleTag tag;
        public bool activated;
        public ObstacleCombo combo;
    }
}