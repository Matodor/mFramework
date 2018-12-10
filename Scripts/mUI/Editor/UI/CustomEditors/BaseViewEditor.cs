using mFramework.UI;
using UnityEditor;

namespace mFramework.Editor.UI
{
    [CustomEditor(typeof(BaseView), true)]
    // ReSharper disable once UnusedMember.Global
    public class BaseViewEditor : UIViewEditor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
        }
    }
}