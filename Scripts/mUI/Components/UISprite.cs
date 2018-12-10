using System.IO;
using System.Linq;
using mFramework.UI.Interfaces;
using UnityEditor;
using UnityEngine;

namespace mFramework.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class UISprite : UIComponent, IRenderer<SpriteRenderer>, IRenderer
    {
        public SpriteRenderer Renderer { get; private set; }
        Renderer IRenderer.Renderer => Renderer;

        protected UISprite()
        {
        }

        protected override void Awake()
        {
            base.Awake();
            Renderer = GetComponent<SpriteRenderer>();
        }

        [ContextMenu("Initialization")]
        public void test()
        {
            Debug.Log($"asd={Application.streamingAssetsPath}");
            
            AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "ui"));
#if UNITY_EDITOR
            
            //AssetImporter.GetAtPath("").SetAssetBundleNameAndVariant();
            Debug.Log($"names={AssetDatabase.GetAllAssetBundleNames().Aggregate((s1, s2) => s1 + "," + s2)}");
            Debug.Log($"path={AssetDatabase.GetAssetPath(Renderer.sprite)}");
#endif
            //AssetBundle.LoadFromFile(")").
        }
    }
}