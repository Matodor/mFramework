using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    [InitializeOnLoad]
    public static class mUIEditor
    {
        static mUIEditor()
        {
            Debug.Log("mUIEditor1");
            Debug.Log("mUIEditor2");

            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
            EditorApplication.hierarchyChanged += EditorApplicationOnHierarchyChanged;
            EditorApplication.update += Update;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange state)
        {
            Debug.Log($"state = {state}");
        }

        private static void Update()
        {
        }

        private static void EditorApplicationOnHierarchyChanged()
        {
            //Debug.Log("EditorApplicationOnHierarchyChanged");
        }
    }
}
