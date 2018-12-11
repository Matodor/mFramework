using System;
using System.Linq;
using System.Reflection;
using mFramework.Core.Interfaces;
using UnityEngine;

namespace mFramework.Core.Common
{
    public class ComponentWriter
    {
        private readonly string _identifier;
        private readonly Component _component;
        private readonly IClassWriter _;

        public ComponentWriter(string identifier,
            Component component, IClassWriter writer)
        {
            _identifier = identifier;
            _component = component;
            _ = writer;

            var cachedType = mCore.GetCachedType(component.GetType());
            foreach (var cachedField in cachedType.CachedFields)
            {
                if (!cachedField.FieldInfo.IsPublic)
                    continue;

                WriteValue(cachedField);
            }

            foreach (var cachedProperty in cachedType.CachedProperties)
            {
                if (!cachedProperty.PropertyInfo.CanRead || 
                    !cachedProperty.PropertyInfo.CanWrite ||
                    !cachedProperty.PropertyInfo.GetMethod.IsPublic ||
                    !cachedProperty.PropertyInfo.SetMethod.IsPublic)
                    continue;
                
                WriteValue(cachedProperty);
            }
        }

        private void WriteValue(ICachedMemberInfo cached)
        {
            if (cached.MemberInfo.GetCustomAttributes<ObsoleteAttribute>().Any())
                return;

            var type = (cached.MemberInfo as PropertyInfo)?.PropertyType ??
                       (cached.MemberInfo as FieldInfo)?.FieldType;

            var value = cached.GetValue(_component);
            if (value == null)
                return;

            Debug.Log($"Ref={type}\n{_component.GetType().Name}.{cached.MemberInfo.Name} = {cached.GetValue(_component)}");
            _.Comment(value.ToString());
        }

        private string StringValue(object value)
        {
            if (value is string)
                return $"\"{value}\"";

            return "";
        }
    }
}