using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI.Windows
{
    public class CreateViewWindow : EditorWindow
    {
        private string _namespace;
        private string _viewName;
        private string _savePath;

        [MenuItem("mFramework/mUI/Create view", priority = 10)]
        private static void ShowWindow()
        {
            GetWindow<CreateViewWindow>();
        }

        private void Awake()
        {
            titleContent.text = "New view";
            _namespace = $"{Application.productName}.UI";
            _viewName = "ViewName";
            _savePath = "Scripts/UI";
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            _namespace = EditorGUILayout.TextField("Namespace", _namespace);
            _viewName = EditorGUILayout.TextField("View name", _viewName);
            _savePath = EditorGUILayout.TextField("Save path", _savePath);
            GUILayout.EndVertical();

            if (GUILayout.Button("Generate View"))
            {
                ViewClassGenerator.View(
                    @namespace: _namespace,
                    className: _viewName,
                    savePath: _savePath
                );
            }
        }


        private void CreateView()
        {
            //ViewClassGenerator.View();
        }
    }
}