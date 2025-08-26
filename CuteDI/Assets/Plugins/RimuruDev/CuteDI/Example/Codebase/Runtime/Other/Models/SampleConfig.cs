using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    [CreateAssetMenu(menuName = "Create SampleConfig", fileName = "SampleConfig", order = 0)]
    public sealed class SampleConfig : ScriptableObject
    {
        public int Value;
    }
}