using System.Globalization;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    [InitializeOnLoad]
    public static class mEditor
    {
        static mEditor()
        {
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                var definition = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
                definition.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = definition;
            }

            Debug.Log("[mEditor] Attached");

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
