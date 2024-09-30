using System;
using UnityEngine;

namespace Project.Runtime.Scriptable
{
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public static class RarityColors
    {
        private const float R = 255f;
        public static Color Common = new(80 / R, 80 / R, 80 / R, 1f);
        public static Color Uncommon = new(80 / R, 210 / R, 100 / R, 1f);
        public static Color Rare = new(44 / R, 181 / R, 255 / R, 1f);
        public static Color Epic = new(200 / R, 91 / R, 255 / R, 1f);
        public static Color Legendary = new(255 / R, 100 / R, 19 / R, 1f);

        public static Color GetColorByRarity(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Common => Common,
                Rarity.Uncommon => Uncommon,
                Rarity.Rare => Rare,
                Rarity.Epic => Epic,
                Rarity.Legendary => Legendary,
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
    }
}