using UnityEngine;

namespace MusicalRunes
{
    [CreateAssetMenu(fileName = "new Hint Powerup Config", menuName = "Configs/Hint Power Up", order = 0)]
    public class HintPowerupConfig : PowerupConfig
    {
        public int[] hintsPerLevel = { 2, 3, 4 };
        public int GetHintAmount(int level) => hintsPerLevel[level - 1];
    }
}