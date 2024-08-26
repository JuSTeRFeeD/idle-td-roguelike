using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable
{
    public class UniqueConfig : ScriptableObject
    {
        [field:SerializeField] public bool ShowUniqueId { get; private set; }
        
        [GUIColor(0.5f, 0.5f, 0.5f, 1f)]
        [ShowIf("ShowUniqueId")]
        public string uniqueID;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(uniqueID)) return;
            GenerateUniqueId();
        }

        private void GenerateUniqueId()
        {
            uniqueID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            // UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log($"Generated new id for {name}: {uniqueID}");
        }

        [PropertySpace(40f, 40f)]
        [Button(SdfIconType.Bug, IconAlignment.LeftEdge)]
        [ShowIf("ShowUniqueId")]
        private void RegenerateUniqueId()
        {
            GenerateUniqueId();
        }
#endif
    }
}