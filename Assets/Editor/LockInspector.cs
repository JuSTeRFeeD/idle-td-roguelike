using UnityEditor;
using UnityEngine;

public class LockInspector : EditorWindow
{
    private static bool isLocked = false;
    private static Object lastSelectedObject;

    [MenuItem("Tools/Hotkeys/Lock Inspector %l")]
    static void LockInspectorToggle()
    {
        var windowType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var inspectorWindow = EditorWindow.GetWindow(windowType);

        isLocked = !isLocked;
        inspectorWindow.ShowNotification(new GUIContent(isLocked ? "Inspector is locked!" : "Inspector is unlocked!"));

        var lockedProperty = windowType.GetProperty("isLocked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (lockedProperty != null)
        {
            lockedProperty.SetValue(inspectorWindow, isLocked, null);
        }
        inspectorWindow.Focus();
        inspectorWindow.Repaint();
    }
}
