using System;
using UnityEngine;

namespace MusicalRunes
{
    [CreateAssetMenu(fileName = "new Powerup Config", menuName = "Configs/Power Up", order = 0)]
    public class PowerupConfig : ScriptableObject
    {
        public PowerupType powerupType;
        public string powerupNameId;
        public string descriptionId;

        public int[] pricePerLevel = { 50, 100, 200 };
        public int[] cooldownAtLevel = { 5, 4, 3 };
        public bool decreaseCooldownOnRuneActivation;

        public string PowerupName => Localization.GetLocalizedText(powerupNameId);
        public string Description => Localization.GetLocalizedText(descriptionId);

        public int MaxLevel => pricePerLevel.Length;

        public int GetUpgradePrice(int level) => level >= MaxLevel ? Int32.MaxValue : pricePerLevel[level];
        public int GetCooldown(int level) => level == 0 ? 0 : cooldownAtLevel[level - 1];
    }
}