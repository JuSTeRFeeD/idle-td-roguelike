namespace Project.Runtime.Features.Building.Data
{
    [System.Serializable]
    public class BuildingData
    {
        public string id;
        public int gridIdx;
        public float rotY;
        
        /// <summary>
        /// Используется чтобы не сохранять 2х2.
        /// False - реальное расположение объекта
        /// True - для колизии широких построек
        /// </summary>
        [System.NonSerialized]
        public bool IsRootPos;

        /// <summary>
        /// Указывавет какой idx является rootIdx 
        /// </summary>
        [System.NonSerialized]
        public int RootIdx;
    }
}