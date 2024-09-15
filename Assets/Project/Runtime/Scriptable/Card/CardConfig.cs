using System.Collections.Generic;
using Project.Runtime.Scriptable.Card.Perks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card
{
    [CreateAssetMenu(menuName = "Game/Cards/Card")]
    public class CardConfig : UniqueConfig
    {
        [Title("Card info")]
        [PreviewField]
        [SerializeField] private Sprite icon;
        [SerializeField] private string title;
        [SerializeField] private bool isBuilding;
        [Space]
        [Tooltip("How much cards of this type can be dropped during game phase")]
        [SerializeField] private int maxPerGame = 3;

        [Title("Upgrade Params")]
        [SerializeField] private List<PerkConfig> perks = new();

        [HideIf("__isSubActive")]
        [PropertySpace(10)]
        [SerializeField] private List<CardConfig> subCardConfigs = new();

        public Sprite Icon => icon;
        public string Title => title;
        public bool IsBuilding => isBuilding;
        public int MaxPerGame => maxPerGame;
        public List<IPerk> Perks => perks.ConvertAll<IPerk>(perk => perk);
        public List<CardConfig> SubCardConfigs => subCardConfigs;
        
#if UNITY_EDITOR
        
        
        private bool __isRootActive => UnityEditor.AssetDatabase.IsMainAsset(this);
        private bool __isSubActive => !__isRootActive;
        
        [GUIColor(1, 0.6f, 0.4f)]
        [PropertySpace(30)]
        [Button("Remove This Sub Card", ButtonSizes.Medium), ShowIf("__isSubActive")]
        public static void RemoveThisSubAsset()
        {
            // Получаем выбранный объект в проекте
            var selectedObject = UnityEditor.Selection.activeObject;

            // Проверяем, что выбран объект и это под-актив
            if (selectedObject != null && UnityEditor.AssetDatabase.IsSubAsset(selectedObject))
            {
                // Удаление под-актива
                Object.DestroyImmediate(selectedObject, true);

                // Сохранение изменений
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();

                Debug.Log("Sub-Asset removed: " + selectedObject.name);
            }
            else
            {
                Debug.LogWarning("Selected object is not a sub-asset or no object selected.");
            }
        }

        [ShowIf("__isRootActive")]
        [GUIColor("RGB(0, 1, 0)")]
        [PropertySpace(30)]
        public string __SetNameToSubActive;
        
        [GUIColor("RGB(0, 1, 0)")]
        [Button("Create sub card config", ButtonSizes.Medium)]
        [ShowIf("__isRootActive")]
        public void CreateSubCardConfig()
        {
            // Создание связанных объектов CardConfig (улучшений)
            var perkConfig = CreateInstance<CardConfig>();
            perkConfig.name = __SetNameToSubActive;
            
            // Добавление улучшений как вложенных активов
            UnityEditor.AssetDatabase.AddObjectToAsset(perkConfig, this);

            // Установка ссылок на улучшения в основной карточке
            subCardConfigs.Add(perkConfig);

            // Сохранение активов
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            Debug.Log("CardConfig with upgrades created!");
        }
#endif
    }
}