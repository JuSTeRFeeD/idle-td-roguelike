using System.Collections.Generic;
using Project.Runtime.Scriptable;

namespace Project.Runtime.Features.Databases
{
    public abstract class GenericDatabase<T> where T : UniqueConfig
    {
        protected readonly Dictionary<string, T> ItemsById = new();
        
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