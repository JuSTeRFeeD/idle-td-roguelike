using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks
{
    public abstract class PerkConfig : ScriptableObject, IPerk
    {
        public abstract void Apply(World world, int applyIndex);
        public abstract string GetPerkDescription(int applyIndex);
    }
}