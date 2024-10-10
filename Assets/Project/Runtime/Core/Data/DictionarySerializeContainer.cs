using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Runtime.Core.Data
{
    /// Serializable Dictionary Container 
    [Serializable]
    public class DictionarySerializeContainer<TKey, TValue>
    {
        public List<TKey> keys = new();
        public List<TValue> values = new();

        public DictionarySerializeContainer()
        {
        }
        
        public DictionarySerializeContainer(Dictionary<TKey, TValue> dictionary)
        {
            foreach (var pair in dictionary)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Debug.Assert(keys.Count == values.Count, "Keys and Values must be equal quantity");

            var result = new Dictionary<TKey, TValue>(keys.Count);
            for (var i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], values[i]);
            }
            return result;
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }
    }
}