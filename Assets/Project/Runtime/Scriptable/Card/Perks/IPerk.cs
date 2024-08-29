using Scellecs.Morpeh;

namespace Project.Runtime.Scriptable.Card.Perks
{
    public interface IPerk
    {
        public void Apply(World world, int applyIndex);
        public string GetPerkDescription(int applyIndex);
    }
}