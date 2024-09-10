using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace bl4st.TimeScaleToolbar
{
    [InitializeOnLoad]
    public class TimeScaleToolbar
    {

        public static readonly string key_enabled = "TimeScaleToolbar_Enabled";
        public static readonly string key_timeScale = "TimeScaleToolbar_TimeScale";
        public static readonly string key_maxScale = "TimeScaleToolbar_Max";
        public static readonly string key_toolbarPosition = "TimeScaleToolbar_Position";
        public static readonly string key_toolbarOffset = "TimeScaleToolbar_Offset";
        public static readonly string[] toolbarPositions = { "Left", "Right" };

        public static float timeScale;
        public static float maxScale;
        public static int toolbarPosition = 1;
        public static int toolbarOffset;
        public static bool enabled = true;

        private static readonly float _sliderWidth = 200f;
        private const int totalWidth = 350;


        static TimeScaleToolbar()
        {
            Initialize();
            if(enabled)
                UpdateDrawCallbacks();
        }

        static void UpdateDrawCallbacks()
        {
            if (toolbarPosition == 0)
            {
                ToolbarExtender.RightToolbarGUI.Remove(OnToolbarGUI);
                ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
            }
            else
            {
                ToolbarExtender.LeftToolbarGUI.Remove(OnToolbarGUI);
                ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
            }
        }

        static void SetVisibility()
        {
            if (enabled)
                UpdateDrawCallbacks();
            else
            {
                ToolbarExtender.LeftToolbarGUI.Remove(OnToolbarGUI);
                ToolbarExtender.RightToolbarGUI.Remove(OnToolbarGUI);
            }
            SceneView.RepaintAll();
        }

        private static void Initialize()
        {
            enabled = EditorPrefs.GetBool(key_enabled, true);
            maxScale = EditorPrefs.GetFloat(key_maxScale, 2f);
            timeScale = Mathf.Min(EditorPrefs.GetFloat(key_timeScale, 1f), maxScale);
            toolbarPosition = EditorPrefs.GetInt(key_toolbarPosition, 1);
            toolbarOffset = EditorPrefs.GetInt(key_toolbarOffset, 10);

            EditorPrefs.SetFloat(key_timeScale, timeScale);
            EditorPrefs.SetBool(key_enabled, enabled);
            EditorPrefs.SetFloat(key_maxScale, maxScale);
            EditorPrefs.SetInt(key_toolbarOffset, toolbarOffset);
        }

        static void OnToolbarGUI()
        {
            GUILayout.Space(toolbarOffset);
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(1));

            GUILayout.Label("TimeScale", GUILayout.Width(70));

            float oldScale = timeScale;
            timeScale = GUILayout.HorizontalSlider(timeScale, 0f, maxScale, GUILayout.Width(_sliderWidth));

            GUILayout.Space(4);

            GUILayout.Label(timeScale.ToString("F2"), GUILayout.Width(45));

            if (GUILayout.Button("Reset", GUILayout.Width(50)))
                timeScale = 1f;

            Time.timeScale = timeScale;

            GUILayout.EndHorizontal();

            if (oldScale != timeScale)
                EditorPrefs.SetFloat(key_timeScale, timeScale);
        }

        [SettingsProvider]
        public static SettingsProvider MyTimeScaleToolbarSettingsProvider()
        {
            var provider = new SettingsProvider("Project/TimeScaleToolbarSettingsProvider", SettingsScope.Project)
            {
               
                label = "TimeScale Toolbar",
                guiHandler = (searchContext) =>
                {
                    bool oldEnable = enabled;
                    enabled = EditorGUILayout.Toggle("Enable", enabled);
                    if (oldEnable != enabled)
                        SetVisibility();

                    int oldPos = toolbarPosition;
                    toolbarPosition = EditorGUILayout.Popup("Toolbar Position", toolbarPosition, toolbarPositions);
                    if (oldPos != toolbarPosition && enabled)
                        UpdateDrawCallbacks();

                    int oldOffset = toolbarOffset;
                    toolbarOffset = EditorGUILayout.IntSlider("Position Offset", toolbarOffset, 0, Screen.width);

                    maxScale = Mathf.Clamp(EditorGUILayout.FloatField("Maximum TimeScale", maxScale), 1f, 100f);

                    GUIStyle italicStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontStyle = FontStyle.Italic
                    };

                    EditorGUILayout.Space(15);
                    EditorGUILayout.LabelField("If the toolbar is not properly updated, press any key", italicStyle);
                    EditorGUILayout.LabelField("or hover the mouse above the toolbar", italicStyle);

                    EditorPrefs.SetBool(key_enabled, enabled);
                    EditorPrefs.SetFloat(key_maxScale, maxScale);
                    EditorPrefs.SetInt(key_toolbarPosition, toolbarPosition);
                    EditorPrefs.SetInt(key_toolbarOffset, toolbarOffset);
                }
            };
            return provider;
        }
    }




}
