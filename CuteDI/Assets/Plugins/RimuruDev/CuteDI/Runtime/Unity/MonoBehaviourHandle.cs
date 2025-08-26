using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    // NOTE:
    // Своего рода disposable токен для монобехов.
    // Ничего по лучше пока не придумал.
    // Они по сути пока не так часто нужны.
    // Но все же мне так удобнее ту же шорточку спавнить/UI-Roots и тому подобные вещи.
    //
    // Нужно будет подумать еще над этим. 
    //
    // === === === === //
    [Preserve]
    public sealed class MonoBehaviourHandle<TMonoBehaviour> : IDisposable where TMonoBehaviour : MonoBehaviour
    {
        [Preserve] 
        public TMonoBehaviour Value { get; }

        [Preserve]
        public MonoBehaviourHandle(TMonoBehaviour value) => Value = value;

        [Preserve]
        public void Dispose()
        {
            if (!Value)
                return;

            UnityEngine.Object.Destroy(Value?.gameObject);
        }
    }
}