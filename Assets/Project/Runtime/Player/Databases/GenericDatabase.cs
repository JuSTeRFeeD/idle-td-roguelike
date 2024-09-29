using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Scriptable;

namespace Project.Runtime.Player.Databases
{
    public abstract class GenericDatabase<T> where T : UniqueConfig
    {
        protected readonly Dictionary<string, T> ItemsById = new();

        public IEnumerable<T> GetAllItems()
        {
            return ItemsById.Select(i => i.Value);
        }
        
        public T GetById(string id)
        {
            return ItemsById.GetValueOrDefault(id);
        }
        
        public bool TryGetById(string id, out T itemData)
        {
            itemData = ItemsById.GetValueOrDefault(id);
            return itemData != null;
        }
    }
}