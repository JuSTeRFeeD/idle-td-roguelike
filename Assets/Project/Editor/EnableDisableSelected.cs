using UnityEditor;
using UnityEngine;

public class EnableDisableSelected : MonoBehaviour
{
    [MenuItem("Tools/Hotkeys/Toggle Activation %#e")]
    private static void ToggleActivation()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length > 0)
        {
            foreach (GameObject obj in selectedObjects)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
}