namespace Project.Runtime.Services.Saves
{
    public interface ISaveManager
    {
        public void Save(bool force);
        public void Load();
    }
}