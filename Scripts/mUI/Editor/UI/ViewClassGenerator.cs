// ReSharper disable ConvertToLocalFunction
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    public class ViewClassGenerator
    {
        public static string BasePath => Application.dataPath;

        private delegate void InsertContent();

        private StreamWriter _writer;
        private int _indentLevel;

        private ViewClassGenerator()
        {

        }

        public static void View(string nameSpace, string className,
            string savePath)
        {
            new ViewClassGenerator().CreateView(nameSpace, className, savePath, null, null);
        }

        public static void View(UIView view)
        {
            new ViewClassGenerator().FillView(view);
        }

        private void FillView(UIView view)
        {
            var childs = new Dictionary<string, UIObject>();
            for (var i = 0; i < view.transform.childCount; i++)
            {
                var child = view.transform.GetChild(i).GetComponent<UIObject>();
                if (child == null)
                    continue;

                var identifier = ParseIdentifier(child.name);
                if (string.IsNullOrWhiteSpace(identifier))
                    throw new Exception("Invalid identifier");

                var repeats = 0;
                var cleanIdentifier = identifier;

                while (childs.ContainsKey(identifier))
                {
                    repeats++;
                    identifier = cleanIdentifier + repeats;
                }

                childs.Add(identifier, child);
            }

            InsertContent afterProps = () =>
            {
                NewLine();
                DirectiveRegion("Components");
                foreach (var pair in childs)
                {
                    AutoProperty(
                        type: pair.Value.GetType().FullName,
                        name: pair.Key,
                        setterVisibility: "protected");
                }
                DirectiveEndRegion();
            };

            InsertContent afterCtor = () =>
            {
                NewLine();
                OverrideMethod("Awake", content: () =>
                {
                    Line("Initialize();");
                });
                NewLine();
                OverrideMethod("Initialize", content: () =>
                {
                    foreach (var pair in childs)
                    {
                        CreateUIObject(pair.Key, pair.Value.GetType());
                        SetObjectName(pair.Key, pair.Key);
                    }
                });
                // TODO 
                //NewLine();
                //OverrideMethod("OnDestroy", content: () =>
                //{
                //    DirectiveIf("UNITY_EDITOR");
                //    DirectiveElse();
                //    foreach (var pair in childs)
                //    {
                //        Tab(1); Line($"Destroy({pair.Key});");
                //    }
                //    DirectiveEndIf();

                //    foreach (var pair in childs)
                //    {
                //        If($"{pair.Key} != null");
                //        OpeningBkt();
                //        {
                //            DirectiveIf("UNITY_EDITOR");
                //            If("Application.isPlaying");
                //            Tab(1); Line($"Destroy({pair.Key});");
                //            Else();
                //            Tab(1); Line($"DestroyImmediate({pair.Key});");
                //            DirectiveElse();
                //            Line($"Destroy({pair.Key});");
                //            DirectiveEndIf();
                //        }
                //        ClosingBkt();
                //    }
                //});
            };

            CreateView(
                nameSpace: view.Namespace,
                className: view.GetType().Name,
                savePath: view.SavePath,
                afterProps: afterProps,
                afterCtor: afterCtor
            );
        }

        private void CreateView(string nameSpace, string className, 
            string savePath, InsertContent afterProps, InsertContent afterCtor)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                Debug.LogError("View name cannot be null or whitespace");
                return;
            }

            if (string.IsNullOrWhiteSpace(nameSpace))
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
                    using (_writer = new StreamWriter(stream))
                    {
                        BaseStructure();
                        NewLine();
                        Namespace(nameSpace);
                        OpeningBkt();
                        {
                            ClassName(className, nameof(UIView));
                            OpeningBkt();
                            {
                                DirectiveIf("UNITY_EDITOR");
                                DirectiveRegion("EDITOR");

                                OverrideAutoProperty("string", 
                                    nameof(UIView.SavePath), $"\"{savePath}\"");
                                OverrideAutoProperty("string", 
                                    nameof(UIView.Namespace), $"\"{nameSpace}\"");

                                DirectiveEndRegion();
                                DirectiveEndIf();

                                afterProps?.Invoke();

                                NewLine();
                                MenuItem(className);
                                NewLine();

                                Line($"private {className}()");
                                OpeningBkt();
                                ClosingBkt();

                                afterCtor?.Invoke();
                            }
                            ClosingBkt();
                        }
                        ClosingBkt();
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

        private static string ParseIdentifier(string identifier)
        {
            var builder = new StringBuilder(identifier.Length);
            for (var i = 0; i < identifier.Length; i++)
            {
                if (builder.Length == 0 && char.IsDigit(identifier[i]))
                    continue;

                if (identifier[i] == ' ')
                    builder.Append('_');
                else if (char.IsLetterOrDigit(identifier[i]))
                    builder.Append(identifier[i]);
            }

            return builder.ToString();
        }

        private void MenuItem(string className)
        {
            DirectiveIf("UNITY_EDITOR");
            Line($"[MenuItem(\"mFramework/mUI/Views/{className}\")]");
            Line("private static void Create()");
            OpeningBkt();
            {
                Line($"var component = mUI.Create<{className}>();");
                Line("Undo.RegisterCreatedObjectUndo(component.gameObject, $\"Create {component.name}\");");
                Line("Selection.activeGameObject = component.gameObject;");
            }
            ClosingBkt();
            DirectiveEndIf();
        }

        private void ClassName(string className, string parentClass)
        {
            Line(string.IsNullOrWhiteSpace(parentClass)
                ? $"public class {className}"
                : $"public class {className} : {parentClass}");
        }

        private void AutoProperty(string type, string name, 
            string value = null, string setterVisibility = null)
        {
            if (string.IsNullOrWhiteSpace(setterVisibility))
                setterVisibility = "";
            else
                setterVisibility = setterVisibility + " ";

            Line(value != null
                ? $"public {type} {name} {{ get; {setterVisibility}set; }} = {value};"
                : $"public {type} {name} {{ get; {setterVisibility}set; }}");
        }

        private void OverrideAutoProperty(string type, string name, 
            string value = null, string setterVisibility = null)
        {
            Line(value != null
                ? $"public override {type} {name} {{ get; {setterVisibility ?? ""}set; }} = {value};"
                : $"public override {type} {name} {{ get; {setterVisibility ?? ""}set; }}");
        }

        private void Method(string name, string visibility = "protected",
            string type = "void", InsertContent content = null, bool virutal = false)
        {
            Line($"{visibility} {(virutal ? "virtual " : "")}{type} {name}()");
            OpeningBkt();
            {
                content?.Invoke();
            }
            ClosingBkt();
        }

        private void OverrideMethod(string name, string visibility = "protected", 
            string type = "void", InsertContent content = null)
        {
            Line($"{visibility} override {type} {name}()");
            OpeningBkt();
            {
                Line($"base.{name}();");
                NewLine();
                content?.Invoke();
            }
            ClosingBkt();
        }

        private void CreateUIObject(string identifier, Type type)
        {
            Line($"{identifier} = mUI.Create<{type.FullName}>(this);");
        }

        private void SetObjectName(string identifier, string value)
        {
            Line($"{identifier}.name = \"{value}\";");
        }

        private void Else()
        {
            Line("else");
        }

        private void If(string condition)
        {
            Line($"if ({condition})");
        }

        private void DirectiveRegion(string region)
        {
            Line($"#region {region}");
        }

        private void DirectiveEndRegion()
        {
            Line("#endregion");
        }

        private void DirectiveElse()
        {
            _writer.WriteLine("#else");
        }

        private void DirectiveEndIf()
        {
            _writer.WriteLine("#endif");
        }

        private void DirectiveIf(string directive)
        {
            _writer.WriteLine($"#if {directive}");
        }

        private void NewLine()
        {
            _writer.WriteLine("");
        }

        private void ClosingBkt()
        {
            _indentLevel--;
            Line("}");
        }

        private void OpeningBkt()
        {
            Line("{");
            _indentLevel++;
        }

        private void BaseStructure()
        {
            Comment("AUTOGENERATED! NOT MODIFY THIS FILE");
            Using("mFramework.UI");
            Using("UnityEngine");
            DirectiveIf("UNITY_EDITOR");
            Using("UnityEditor");
            DirectiveEndIf();
        }

        private void Comment(string comment)
        {
            Line($"/* {comment} */");
        }

        private void Namespace(string @namespace)
        {
            Line($"namespace {@namespace}");
        }

        private void Using(string @namespace)
        {
            Line($"using {@namespace};");
        }

        private void Line(string text)
        {
            Tab(_indentLevel);
            _writer.WriteLine(text);
        }

        private void Tab(int indentLevel)
        {
            if (indentLevel > 0)
                _writer.Write(new string('\t', indentLevel));
        }

        private static void CheckDirectory(string path)
        {
            if (Directory.Exists(path))
                return;
            Directory.CreateDirectory(path);
        }
    }
}