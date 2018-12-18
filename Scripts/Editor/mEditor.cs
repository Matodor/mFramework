using System.Globalization;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor
{
    // ReSharper disable once UnusedMember.Global
    [InitializeOnLoad]
    public sealed class mEditor
    {
        public static mEditor Instance { get; }

        static mEditor()
        {
            Instance = new mEditor();
        }

        private mEditor()
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
        }

        ~mEditor()
        {
            EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
            EditorApplication.hierarchyChanged -= EditorApplicationOnHierarchyChanged;
            Debug.Log("[mEditor] ~mEditor");
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange state)
        {
            Debug.Log($"state = {state}");
        }

        private static void EditorApplicationOnHierarchyChanged()
        {
        }

        // ReSharper disable once UnusedMember.Local
        [MenuItem("Assets/Build AssetBundles")]
        private static void BuildAllAssetBundles()
        {
            const string folderName = "AssetBundles";
            var filePath = Path.Combine(Application.streamingAssetsPath, folderName);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }
    }
}
