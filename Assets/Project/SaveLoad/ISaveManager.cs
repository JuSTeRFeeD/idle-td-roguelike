namespace Project.SaveLoad
{
    public interface ISaveManager
    {
        public void Save(PersistentPlayerData playerData);
        public PersistentPlayerData Load();
    }
}