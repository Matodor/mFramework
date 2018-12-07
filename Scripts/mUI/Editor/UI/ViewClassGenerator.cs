// ReSharper disable ConvertToLocalFunction
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using mFramework.Editor.Extensions;
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

                var childView = child as UIView;
                if (childView != null)
                {
                    if (view.GetType() == childView.GetType())
                        throw new Exception("Nested views of same type are prohibited");

                    View(childView);
                }

                var identifier = ParseIdentifier(child.name);
                if (string.IsNullOrWhiteSpace(identifier))
                    throw new Exception("Invalid identifier");

                if (identifier == child.GetType().Name)
                    identifier = $"{child.GetType().Name}1";

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
                    DirectiveRegion("Initialize components");
                    foreach (var pair in childs)
                    {
                        InitializeObject(pair.Key, pair.Value);
                    }
                    DirectiveEndRegion();
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

        private void InitializeObject(string identifier, UIObject value)
        {
            CreateUIObject(identifier, value.GetType());
            SetObjectName(identifier, identifier);
            SetHideFlags(identifier, HideFlags.DontSave);
            SetLocalPosition(identifier, value.transform.localPosition);
            SetLocalEulerAngles(identifier, value.transform.localEulerAngles);
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

            if (className.Contains("View") == false)
                className = className + "View";

            var path = Path.GetFullPath(Path.Combine(BasePath, savePath));
            CheckDirectory(path);
            path = Path.Combine(path, $"{className}.cs");
            Debug.Log($"Generate view: path={path}");

            try
            {
                using (var stream = new MemoryStream())
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

                        _writer.Flush();

                        try
                        {
                            using (var fileStream = File.Create(path))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.CopyTo(fileStream);
                            }
                        }
                        catch (Exception exception)
                        {
                            Debug.LogException(exception);
                            throw;
                        }
                    }
                }

                _writer = null;
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
            Line($"[MenuItem(\"mFramework/mUI/Views/Generated/{className}\")]");
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

        private void SetLocalEulerAngles(string identifier, Vector3 v)
        {
            Line($"{identifier}.transform.localEulerAngles = {v.String()};");
        }

        private void SetLocalPosition(string identifier, Vector3 v)
        {
            Line($"{identifier}.transform.localPosition = {v.String()};");
        }

        private void SetHideFlags(string identifier, HideFlags value)
        {
            Line($"{identifier}.gameObject.hideFlags = HideFlags.{value.ToString()};");
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