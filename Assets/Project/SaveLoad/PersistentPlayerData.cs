using Project.Runtime.Player;

namespace Project.SaveLoad
{
    public class PersistentPlayerData
    {
        public string MapData; // LevelMapGenerator.cs Serialization
        public int CurMapPointIndex;
        
        public readonly Wallet HardCurrency = new();
        public readonly Wallet SoftCurrency = new();
    }
}