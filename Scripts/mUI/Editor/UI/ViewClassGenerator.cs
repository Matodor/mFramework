using System;
using System.IO;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    public static class ViewClassGenerator
    {
        public static string BasePath => Application.dataPath;

        public static void View(UIView view)
        {

        } 

        public static void View(string @namespace, string className, 
            string savePath)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                Debug.LogError("View name cannot be null or whitespace");
                return;
            }

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                Debug.LogError("View namespace cannot be null or whitespace");
                return;
            }

            if (string.IsNullOrWhiteSpace(savePath))
            {
                Debug.LogError("Setup view save path");
                return;
            }

            var path = Path.GetFullPath(Path.Combine(BasePath, savePath));
            CheckDirectory(path);
            path = Path.Combine(path, $"{className}.cs");
            Debug.Log($"Generate view: path={path}");

            try
            {
                using (var stream = File.Create(path))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var indentLevel = 0;
                        writer.BaseStructure();
                        writer.NewLine();
                        writer.Namespace(@namespace);
                        writer.OpeningBkt(indentLevel);
                        {
                            indentLevel++;
                            writer.ClassName(className, nameof(UIView), indentLevel);
                            writer.OpeningBkt(indentLevel);
                            {
                                indentLevel++;
                                writer.DirectiveIf("UNITY_EDITOR");
                                writer.DirectiveRegion("EDITOR", indentLevel);

                                writer.OverrideAutoProperty("string", 
                                    nameof(UIView.GeneratePath), indentLevel, $"\"{savePath}\"");
                                writer.OverrideAutoProperty("string", 
                                    nameof(UIView.Namespace), indentLevel, $"\"{@namespace}\"");

                                writer.DirectiveEndRegion(indentLevel);
                                writer.DirectiveEndIf();

                                writer.NewLine();
                                writer.MenuItem(className, indentLevel);
                                writer.NewLine();

                                writer.Line($"private {className}()", indentLevel);
                                writer.OpeningBkt(indentLevel);
                                writer.ClosingBkt(indentLevel);
                                indentLevel--;
                            }
                            writer.ClosingBkt(indentLevel);
                            indentLevel--;
                        }
                        writer.ClosingBkt(indentLevel);
                    }
                }

                AssetDatabase.Refresh();
                Debug.Log("Successful");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void MenuItem(this StreamWriter writer, 
            string className, int indentLevel = 0)
        {
            writer.DirectiveIf("UNITY_EDITOR");
            writer.Line($"[MenuItem(\"mFramework/mUI/Views/{className}\")]", indentLevel);
            writer.Line("private static void Create()", indentLevel);
            writer.OpeningBkt(indentLevel);
            writer.Line($"var component = mUI.View<{className}>();", indentLevel + 1);
            writer.Line("Undo.RegisterCreatedObjectUndo(component.gameObject, $\"Create {component.name}\");", indentLevel + 1);
            writer.Line("Selection.activeGameObject = component.gameObject;", indentLevel + 1);
            writer.ClosingBkt(indentLevel);
            writer.DirectiveEndIf();
        }

        private static void ClassName(this StreamWriter writer, 
            string className, string parentClass, int indentLevel = 0)
        {
            writer.Tab(indentLevel);
            writer.WriteLine(string.IsNullOrWhiteSpace(parentClass)
                ? $"public class {className}"
                : $"public class {className} : {parentClass}");
        }

        private static void OverrideAutoProperty(this StreamWriter writer, 
            string type, string name, int indentLevel = 0, string value = null)
        {
            writer.Tab(indentLevel);
            writer.WriteLine(value != null
                ? $"public override {type} {name} {{ get; set; }} = {value};"
                : $"public override {type} {name} {{ get; set; }}");
        }

        private static void DirectiveRegion(this StreamWriter writer, 
            string region, int indentLevel = 0)
        {
            writer.Tab(indentLevel);
            writer.WriteLine($"#region {region}");
        }

        private static void DirectiveEndRegion(this StreamWriter writer, int indentLevel = 0)
        {
            writer.Tab(indentLevel);
            writer.WriteLine("#endregion");
        }

        private static void DirectiveEndIf(this StreamWriter writer)
        {
            writer.WriteLine("#endif");
        }

        private static void DirectiveIf(this StreamWriter writer, string directive)
        {
            writer.WriteLine($"#if {directive}");
        }

        private static void Line(this StreamWriter writer, string line, int indentLevel = 0)
        {
            writer.Tab(indentLevel);
            writer.WriteLine(line);
        }

        private static void NewLine(this StreamWriter writer)
        {
            writer.WriteLine("");
        }

        private static void ClosingBkt(this StreamWriter writer, int indentLevel = 0)
        {
            writer.Tab(indentLevel);
            writer.WriteLine("}");
        }

        private static void OpeningBkt(this StreamWriter writer, int indentLevel = 0)
        {
            writer.Tab(indentLevel);
            writer.WriteLine("{");
        }

        private static void Tab(this StreamWriter writer, int indentLevel)
        {
            if (indentLevel > 0)
                writer.Write(new string('\t', indentLevel));
        }

        private static void BaseStructure(this StreamWriter writer)
        {
            writer.Comment("AUTOGENERATED! NOT MODIFY THIS FILE");
            writer.Using("mFramework.UI");
            writer.Using("UnityEngine");
            writer.DirectiveIf("UNITY_EDITOR");
            writer.Using("UnityEditor");
            writer.DirectiveEndIf();
        }

        private static void Comment(this StreamWriter stream,
            string comment, int indentLevel = 0)
        {
            stream.Tab(indentLevel);
            stream.WriteLine($"/* {comment} */");
        }

        private static void Namespace(this StreamWriter stream, string @namespace)
        {
            stream.WriteLine($"namespace {@namespace}");
        }

        private static void Using(this StreamWriter stream, string @namespace)
        {
            stream.WriteLine($"using {@namespace};");
        }

        private static void CheckDirectory(string path)
        {
            if (Directory.Exists(path))
                return;
            Directory.CreateDirectory(path);
        }
    }
}