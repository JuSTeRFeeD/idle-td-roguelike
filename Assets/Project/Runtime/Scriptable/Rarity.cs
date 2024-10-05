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
    
    public static class RarityExt
    {
        public static string GetRarityName(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Common => "Обычный",
                Rarity.Uncommon => "Необычный",
                Rarity.Rare => "Редкий",
                Rarity.Epic => "Эпический",
                Rarity.Legendary => "Легендарный",
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
        
        private const float R = 255f;
        public static Color Common = new(150 / R, 150 / R, 150 / R, 1f);
        public static Color Uncommon = new(140 / R, 250 / R, 100 / R, 1f);
        public static Color Rare = new(64 / R, 190 / R, 250 / R, 1f);
        public static Color Epic = new(220 / R, 91 / R, 250 / R, 1f);
        public static Color Legendary = new(250 / R, 120 / R, 40 / R, 1f);

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