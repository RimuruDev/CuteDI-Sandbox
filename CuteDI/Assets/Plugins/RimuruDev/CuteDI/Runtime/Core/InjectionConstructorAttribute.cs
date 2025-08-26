using System;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class InjectionConstructorAttribute : Attribute { }

    [Preserve]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class TagAttribute : Attribute
    {
        [Preserve] public string Value { get; }
        [Preserve] public TagAttribute(string value) => Value = value;
    }
}