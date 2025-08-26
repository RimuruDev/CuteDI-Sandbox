using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public interface IFoo { }
    
    public sealed class FooMono : MonoBehaviour, IFoo { }
}