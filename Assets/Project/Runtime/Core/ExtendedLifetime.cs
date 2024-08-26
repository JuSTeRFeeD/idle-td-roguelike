using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Runtime.Core
{
    public class ExtendedLifetime : LifetimeScope
    {
#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button("Find GameObjects to inject")]
        public void Refresh()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                Debug.LogError("You cant refresh object to inject in prefab mode!");
            }
            
            autoInjectGameObjects?.Clear();
            autoInjectGameObjects = FindInjectedGameObjects();
        }

        private List<GameObject> FindInjectedGameObjects()
        {
            var injectedGameObjects = new List<GameObject>();
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (var obj in gameObjects)
            {
                // Sometimes adds prefabs from assets folder. Here we are check to skip this objects
                if (PrefabUtility.IsPartOfPrefabAsset(obj)) continue;

                // Get all components attached to the game object
                var components = obj.GetComponents<Component>();
                foreach (var component in components)
                {
					if (!component) continue;
					
                    // Get all methods in the component, including private ones
                    var methods = component.GetType().GetMethods(
                        BindingFlags.Instance |
                        BindingFlags.NonPublic |
                        BindingFlags.Public | 
                        BindingFlags.FlattenHierarchy);
                    
                    foreach (var method in methods)
                    {
                        if (System.Attribute.IsDefined(method, typeof(InjectAttribute)))
                        {
                            if (!injectedGameObjects.Contains(obj))
                                injectedGameObjects.Add(obj);
                            break;
                        }
                    }

                    // Get all fields in the component, including private ones
                    var fields = component.GetType().GetFields(
                        BindingFlags.Instance |
                        BindingFlags.NonPublic |
                        BindingFlags.Public | 
                        BindingFlags.FlattenHierarchy);
                    
                    foreach (var field in fields)
                    {
                        if (System.Attribute.IsDefined(field, typeof(InjectAttribute)))
                        {
                            if (!injectedGameObjects.Contains(obj))
                                injectedGameObjects.Add(obj);
                            break;
                        }
                    }
                }
            }

            return injectedGameObjects;
        }
#endif
    }
}