using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    internal static class DiActivator
    {
        [Preserve]
        public static object CreateInstance(Type type, DIContainer container)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type))
                return ScriptableObject.CreateInstance(type);

            if (typeof(MonoBehaviour).IsAssignableFrom(type))
                throw new InvalidOperationException($"Cannot auto-construct MonoBehaviour {type.FullName}");

            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (ctors.Length == 0)
                throw new InvalidOperationException($"No public constructors for {type.FullName}");

            var marked = ctors.FirstOrDefault(c => c.GetCustomAttribute<InjectionConstructorAttribute>() != null);

            var ctor = marked ?? ctors.OrderByDescending(c => c.GetParameters().Length).First();

            var parms = ctor.GetParameters();

            var args = new object[parms.Length];
            for (var i = 0; i < parms.Length; i++)
            {
                var p = parms[i];
                var tagAttr = p.GetCustomAttribute<TagAttribute>();
                var tag = tagAttr?.Value;
                args[i] = container.Resolve(p.ParameterType, tag);
            }

            return ctor.Invoke(args);
        }
    }
}