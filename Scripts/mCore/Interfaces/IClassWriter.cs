#if UNITY_EDITOR
using System;
using mFramework.UI.Interfaces;
using UnityEngine;

namespace mFramework.Core.Interfaces
{
    public interface IClassWriter
    {
        void CreateUIObject(string identifier, Type type);
        void SetLocalEulerAngles(string identifier, Vector3 v);
        void SetLocalPosition(string identifier, Vector3 v);
        void SetHideFlags(string identifier, HideFlags value);
        void SetObjectName(string identifier, string value);

        void SetColor(string identifier, IColored colored);

        void Else();
        void If(string condition);
        void DirectiveRegion(string region);
        void DirectiveEndRegion();
        void DirectiveElse();
        void DirectiveEndIf();
        void DirectiveIf(string directive);
        void NewLine();
        void ClosingBkt();
        void OpeningBkt();
        void Comment(string comment);
        void Line(string text);
        void Tab(int indentLevel);
        void MethodInvoke(string identifier, string method, params string[] args);
    }
}
#endif