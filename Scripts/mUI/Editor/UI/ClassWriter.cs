// ReSharper disable ConvertToLocalFunction
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using mFramework.Core.Extensions;
using mFramework.Core.Interfaces;
using mFramework.UI;
using mFramework.UI.Interfaces;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    public class ClassWriter : IClassWriter
    {
        public static string BasePath => Application.dataPath;

        private delegate void InsertContent();

        private StreamWriter _writer;
        private int _indentLevel;

        private ClassWriter()
        {

        }

        public static void View(string nameSpace, string className,
            string savePath)
        {
            new ClassWriter().CreateView(nameSpace, className, savePath, null, null);
        }

        public static void View(UIView view)
        {
            new ClassWriter().FillView(view);
        }

        private void FillView(UIView view)
        {
            var childs = new Dictionary<string, UIObject>();
            for (var i = 0; i < view.transform.childCount; i++)
            {
                var child = view.transform.GetChild(i).GetComponent<UIObject>();
                if (child == null || child.IgnoreByViewWriter)
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
                OverrideMethod("Initialize", content: () =>
                {
                    InitializeObject("this", view.GetType().Name, view);
                    NewLine();

                    DirectiveRegion("Initialize components");
                    foreach (var pair in childs)
                    {
                        Line($"Initialize_{pair.Key}();");
                    }
                    DirectiveEndRegion();
                });

                foreach (var pair in childs)
                {
                    NewLine();
                    Method($"Initialize_{pair.Key}", virutal: true,
                        content: () =>
                        {
                            CreateUIObject(pair.Key, pair.Value.GetType());
                            InitializeObject(pair.Key, pair.Key, pair.Value);
                        });
                }
            };

            CreateView(
                nameSpace: view.Namespace,
                className: view.GetType().Name,
                savePath: view.SavePath,
                afterProps: afterProps,
                afterCtor: afterCtor
            );
        }

        private void InitializeObject(string identifier, string goName, UIObject value)
        {
            value.GenerateInitialize(this, identifier);
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
            
            try
            {
                var designerPath = Path.Combine(path, $"{className}.Designer.cs");
                Debug.Log($"Generate Designer file: {designerPath}");

                CreateDesignerFile(nameSpace, className, savePath, 
                    afterProps, afterCtor, designerPath);

                var partialPath = Path.Combine(path, $"{className}.cs");
                Debug.Log($"Generate partial view file: {partialPath}");

                if (!File.Exists(partialPath))
                    CreatePartialFile(nameSpace, className, partialPath);

                _writer = null;
                AssetDatabase.Refresh();
                Debug.Log("Successful");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void CreatePartialFile(string nameSpace, string className, string filePath)
        {
            using (var stream = new MemoryStream())
            {
                using (_writer = new StreamWriter(stream))
                {
                    Using("mFramework.UI");
                    Using("UnityEngine");
                    DirectiveIf("UNITY_EDITOR");
                    Using("UnityEditor");
                    DirectiveEndIf();
                    NewLine();
                    Namespace(nameSpace);
                    OpeningBkt();
                    {
                        ClassName(className, nameof(UIView));
                        OpeningBkt();
                        {
                            OverrideMethod("Start", content: () =>
                            {
                                Line("Initialize();");
                            });
                        }
                        ClosingBkt();
                    }
                    ClosingBkt();

                    _writer.Flush();

                    try
                    {
                        using (var fileStream = File.Create(filePath))
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
        }

        private void CreateDesignerFile(string nameSpace, string className, 
            string savePath, InsertContent afterProps, InsertContent afterCtor, 
            string filePath)
        {
            using (var stream = new MemoryStream())
            {
                using (_writer = new StreamWriter(stream))
                {
                    Comment("AUTOGENERATED! NOT MODIFY THIS FILE");
                    Using("mFramework.UI");
                    Using("UnityEngine");
                    DirectiveIf("UNITY_EDITOR");
                    Using("UnityEditor");
                    DirectiveEndIf();
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
                        using (var fileStream = File.Create(filePath))
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
            Line($"[MenuItem(\"GameObject/mUI/Views/Generated/{className}\", false, 0)]");
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
                ? $"public partial class {className}"
                : $"public partial class {className} : {parentClass}");
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

        public void CreateUIObject(string identifier, Type type)
        {
            Line($"{identifier} = mUI.Create<{type.FullName}>(this);");
        }

        public void SetLocalEulerAngles(string identifier, Vector3 v)
        {
            Line($"{identifier}.transform.localEulerAngles = {v.StringCtor()};");
        }

        public void SetLocalPosition(string identifier, Vector3 v)
        {
            Line($"{identifier}.transform.localPosition = {v.StringCtor()};");
        }

        public void SetHideFlags(string identifier, HideFlags value)
        {
            Line($"{identifier}.gameObject.hideFlags = HideFlags.{value.ToString()};");
        }

        public void SetObjectName(string identifier, string value)
        {
            Line($"{identifier}.name = \"{value}\";");
        }

        public void Else()
        {
            Line("else");
        }

        public void If(string condition)
        {
            Line($"if ({condition})");
        }

        public void DirectiveRegion(string region)
        {
            Line($"#region {region}");
        }

        public void DirectiveEndRegion()
        {
            Line("#endregion");
        }

        public void DirectiveElse()
        {
            _writer.WriteLine("#else");
        }

        public void DirectiveEndIf()
        {
            _writer.WriteLine("#endif");
        }

        public void DirectiveIf(string directive)
        {
            _writer.WriteLine($"#if {directive}");
        }

        public void NewLine()
        {
            _writer.WriteLine("");
        }

        public void ClosingBkt()
        {
            _indentLevel--;
            Line("}");
        }

        public void OpeningBkt()
        {
            Line("{");
            _indentLevel++;
        }

        public void Comment(string comment)
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

        public void Line(string text)
        {
            Tab(_indentLevel);
            _writer.WriteLine(text);
        }

        public void Tab(int indentLevel)
        {
            if (indentLevel > 0)
                _writer.Write(new string('\t', indentLevel));
        }

        public void SetColor(string identifier, IColored colored)
        {
            Line($"{identifier}.{nameof(IColored.Color)} = {colored.Color.StringCtor()};");
        }

        public void MethodInvoke(string identifier, string method, params string[] args)
        {
            Line($"{identifier}.{method}({string.Join(",", args)});");
        }

        private static void CheckDirectory(string path)
        {
            if (Directory.Exists(path))
                return;
            Directory.CreateDirectory(path);
        }
    }
}