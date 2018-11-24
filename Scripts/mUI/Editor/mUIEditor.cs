using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    public class TestWindow : EditorWindow
    {
        void OnDisable()
        {
            Debug.Log("OnDisable");
        }

        void OnEnable()
        {
            titleContent.text = "asd";
            Debug.Log("OnEnable");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(new GUIStyle() {alignment = TextAnchor.LowerRight});
            GUILayout.Label(new GUIContent("asd"), new GUIStyle() { fontStyle = FontStyle.BoldAndItalic, stretchWidth = false});
            GUILayout.Label(new GUIContent("asd"), new GUIStyle() { fontStyle = FontStyle.BoldAndItalic, stretchWidth = false});
            GUILayout.Label(new GUIContent("asd"), new GUIStyle() { fontStyle = FontStyle.BoldAndItalic, stretchWidth = false});
            GUILayout.Label(new GUIContent("asd"), new GUIStyle() { fontStyle = FontStyle.BoldAndItalic, stretchWidth = false});
            GUILayout.Label(new GUIContent("asd"), new GUIStyle() { fontStyle = FontStyle.BoldAndItalic, stretchWidth = false});
            GUILayout.Label(new GUIContent("asd"), new GUIStyle() { fontStyle = FontStyle.BoldAndItalic, stretchWidth = false});
            GUILayout.EndHorizontal();
        }
    }

    [InitializeOnLoad]
    public static class mUIEditor
    {
        static mUIEditor()
        {
            Debug.Log("mUIEditor1");
            Debug.Log("mUIEditor2");

            //EditorWindow.GetWindow<TestWindow>();
        }

        private static void Update()
        {
        }
    }
}