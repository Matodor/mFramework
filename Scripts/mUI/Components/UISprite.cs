// ReSharper disable ArrangeAccessorOwnerBody
using mFramework.Core.Extensions;
using mFramework.Core.Interfaces;
using mFramework.UI.Interfaces;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace mFramework.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class UISprite : UIComponent, IColored, IRenderer<SpriteRenderer>, IRenderer
    {
        public Color Color
        {
            get { return Renderer.color; }
            set { Renderer.color = value; }
        }

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

        public void LoadSpriteFromBundle(string bundleName, string assetName)
        {
#if UNITY_EDITOR
            Renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetName);
#endif
        }

#if UNITY_EDITOR
        public override void GenerateInitialize(IClassWriter _, string identifier)
        {
            base.GenerateInitialize(_, identifier);

            _.DirectiveRegion("SpriteRenderer");

            if (Renderer.sprite != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(Renderer.sprite);
                var assetImporter = AssetImporter.GetAtPath(assetPath);

                _.MethodInvoke(identifier, nameof(LoadSpriteFromBundle),
                    $"\"{assetImporter.assetBundleName}\"",
                    $"\"{assetImporter.assetPath}\"");
            }

            _.SetColor(identifier, this);

            if (Renderer.drawMode != SpriteDrawMode.Simple)
            {
                _.Line($"{identifier}.Renderer.drawMode = {nameof(SpriteDrawMode)}.{Renderer.drawMode};");
                _.Line($"{identifier}.Renderer.size = {Renderer.size.StringCtor()};");

                if (Renderer.drawMode == SpriteDrawMode.Tiled)
                {
                    _.Line($"{identifier}.Renderer.tileMode = {nameof(SpriteTileMode)}.{Renderer.tileMode};");
                    if (Renderer.tileMode == SpriteTileMode.Adaptive)
                        _.Line($"{identifier}.Renderer.adaptiveModeThreshold = {Renderer.adaptiveModeThreshold}f;");
                }
            }
            else
            {
                if (Renderer.spriteSortPoint != SpriteSortPoint.Center)
                    _.Line($"{identifier}.Renderer.spriteSortPoint = {nameof(SpriteSortPoint)}.{Renderer.spriteSortPoint};");
            }

            if (Renderer.flipX)
                _.Line($"{identifier}.Renderer.flipX = true;");

            if (Renderer.flipY)
                _.Line($"{identifier}.Renderer.flipY = true;");

            if (Renderer.maskInteraction != SpriteMaskInteraction.None)
                _.Line($"{identifier}.Renderer.maskInteraction = {nameof(SpriteMaskInteraction)}.{Renderer.maskInteraction};");

            _.DirectiveEndRegion();
        }
#endif

        //        [ContextMenu("Initialization")]
        //        public void test()
        //        {
        //            Debug.Log($"asd={Application.streamingAssetsPath}");

        //            AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "ui"));
        //#if UNITY_EDITOR

        //            //AssetImporter.GetAtPath("").SetAssetBundleNameAndVariant();
        //            Debug.Log($"names={AssetDatabase.GetAllAssetBundleNames().Aggregate((s1, s2) => s1 + "," + s2)}");
        //            Debug.Log($"path={AssetDatabase.GetAssetPath(Renderer.sprite)}");
        //#endif
        //            //AssetBundle.LoadFromFile(")").
        //        }
    }
}