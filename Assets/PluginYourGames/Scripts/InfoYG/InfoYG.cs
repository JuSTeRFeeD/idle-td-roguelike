using System.IO;
using UnityEditor;
using UnityEngine;
using YG.Insides;

namespace YG
{
    //[CreateAssetMenu(fileName = "SettingsYG2", menuName = "ToolsYG2/Create SettingsYG2")]
    public partial class InfoYG : ScriptableObject
    {
        public static InfoYG instance;
        public static InfoYG Inst()
        {
            if (instance == null)
            {
                InfoYG infoRes = Resources.Load<InfoYG>(NAME_INFOYG_FILE);

#if UNITY_EDITOR
                if (infoRes == null)
                {
                    InfoYG infoYG = ScriptableObject.CreateInstance<InfoYG>();
                    string path = $"{PATCH_ASSETS_YG2}/Resources/{NAME_INFOYG_FILE}.asset";
                    string directory = $"{PATCH_ASSETS_YG2}/Resources";

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    AssetDatabase.CreateAsset(infoYG, path);
                    AssetDatabase.Refresh();
                    infoRes = Resources.Load<InfoYG>(NAME_INFOYG_FILE);

                    instance = infoRes;

                    bool autoApplySettings = EditorUtility.DisplayDialog($"Оптимальные настройки",
                        "Установить оптимальные настройки проекта для платформы по умолчанию «Яндекс Игры»? (Рекомендуется)\n\nShould I set the optimal project settings for the default Yandex Games platform? (Recommended)",
                        "Yes",
                        "No");
                    instance.Basic.autoApplySettings = autoApplySettings;

                    SetDefaultPlatform();
                }
#else
                if (infoRes == null)
                    Debug.LogError($"{NAME_INFOYG_FILE} not found!");
#endif
                instance = infoRes;
            }

            return instance;
        }

        public ProjectSettings platformOptions { get => Basic.platform.projectSettings; }

        public static string CurrentPlatformOrigName()
        {
            PlatformSettings platform = Inst().Basic.platform;
            if (platform != null)
                return platform.nameBase;
            return string.Empty;
        }

#if UNITY_EDITOR
        public static void SetDefaultPlatform()
        {
            string standartPlatformSettingsPath = $"{PATCH_ASSETS_PLATFORMS}/YandexGames/YandexGames.asset";
            PlatformSettings standartPlatformSettings = AssetDatabase.LoadAssetAtPath<PlatformSettings>(standartPlatformSettingsPath);

            if (standartPlatformSettings != null)
            {
                instance.Basic.platform = standartPlatformSettings;

                if (YG2.infoYG.Basic.autoApplySettings)
                    instance.Basic.platform.ApplyProjectSettings();

                EditorUtility.SetDirty(instance);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}
