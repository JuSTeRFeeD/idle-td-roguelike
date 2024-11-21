using UnityEditor;
using UnityEngine;

namespace YG.Insides
{
    [CreateAssetMenu(fileName = "NewPlatformYG", menuName = "YG2/New Platform")]
    public partial class PlatformSettings : ScriptableObject
    {
        public string nameDefining = "NewPlatform";

        public string nameBase { get => nameDefining.Replace("Platform", string.Empty); }

        public ProjectSettings projectSettings = new ProjectSettings();
#if UNITY_EDITOR
        public bool applySettingsBySwitchPlatform = true;

        public void ApplyProjectSettings()
        {
            projectSettings.ApplySettings();

            CallAction.CallIByAttribute(typeof(ApplySettingsAttribute), typeof(CommonOptions), YG2.infoYG.common);

            if (YG2.infoYG.platformToggles.selectWebGLTemplate && projectSettings.selectWebGLTemplate)
            {
                string templateName = nameBase;
                string templatePath = $"Assets/WebGLTemplates/{templateName}";

                if (AssetDatabase.IsValidFolder(templatePath))
                    PlayerSettings.WebGL.template = "PROJECT:" + templateName;
            }
        }

        public static void SelectPlatform()
        {
            CallAction.CallIByAttribute(typeof(SelectPlatformAttribute), typeof(CommonOptions), YG2.infoYG.common);
        }
        public static void DeletePlatform()
        {
            CallAction.CallIByAttribute(typeof(DeletePlatformAttribute), typeof(CommonOptions), YG2.infoYG.common);
        }
#endif
    }
}